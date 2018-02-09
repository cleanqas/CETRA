using AspNet.Identity.MySQL;
using CETRA.Models;
using Microsoft.AspNet.Identity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CETRA.Controllers
{
    public class HOOperationController : Controller
    {
        public UploadManager<UploadEntity> UploadManager;
        public UploadDataManager<UploadDataEntity> UploadDataManager;

        public HOOperationController() :
            this(new UploadManager<UploadEntity>(new UploadStore<UploadEntity>(new ApplicationDbContext())),
                new UploadDataManager<UploadDataEntity>(new UploadDataStore<UploadDataEntity>(new ApplicationDbContext())))
        {
        }

        public HOOperationController(UploadManager<UploadEntity> uploadManager,
            UploadDataManager<UploadDataEntity> uploadDataManager)
        {
            UploadManager = uploadManager;
            UploadDataManager = uploadDataManager;
        }

        [Authorize(Roles = "HeadOfficeOperator")]
        public ActionResult Index()
        {
            return View();
        }

        //POST: /HOOperator/GetAllProcessedUploads
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllProcessedUploads()
        {
            var allProcessedUploads = await new UploadStore<UploadEntity>(new ApplicationDbContext()).FindUploadsByStatusAsync(2);
            List<UploadEntityModel> allProcessedUploadsWithDetails = new List<UploadEntityModel>();
            foreach (var data in allProcessedUploads)
            {
                var branchDetail = await Helper.GetBranchNameAndCode(data.BranchId);
                allProcessedUploadsWithDetails.Add(new UploadEntityModel()
                {
                    BankId = data.BankId,
                    BankName = await Helper.GetBankName(data.BankId),
                    BranchId = data.BranchId,
                    BranchName = branchDetail["BranchName"],
                    BranchCode = branchDetail["BranchCode"],
                    Id = data.Id,
                    OperatorId = data.OperatorId,
                    Status = data.Status,
                    UploadDate = data.UploadDate,
                    UploaderId = data.UploaderId
                });
            }
            return Json(new { data = allProcessedUploadsWithDetails }, JsonRequestBehavior.AllowGet);
        }

        //POST: /HOOperator/GetAllUploadDataDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllUploadDataDetail(string UploadId)
        {
            var uploadData = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).GetUploadsDataWithAccountName(UploadId);
            return Json(uploadData, JsonRequestBehavior.AllowGet);
        }

        //POST: /HOOperator/MarkUploadAsProcessed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MarkUploadAsProcessed(string UploadId)
        {
            var uploadStore = new UploadStore<UploadEntity>(new ApplicationDbContext());
            await uploadStore.UpdateUploadStatus(UploadId, 3);
            await uploadStore.UpdateUploadHOProcessor(UploadId, User.Identity.GetUserId());
            return Json(new { code = "00", message = "Successful" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ConfirmUpload(UploadDataModel model)
        {
            if (ModelState.IsValid & model != null)
            {
                var upload = new UploadEntity(User.Identity.GetUserId(), model.BankId, model.BranchId, 0);
                bool result = false;
                try
                {
                    result = await UploadManager.CreateAsync(upload);
                }
                catch (Exception ex)
                {
                    throw new HttpException(400, ex.Message);
                }
                if (result)
                {
                    try
                    {
                        foreach (var data in model.PData)
                        {
                            var uploaddata = new UploadDataEntity(upload.Id, data.Narration, data.Amount, data.AccountNumber, data.Debit1Credit0, data.PostingCode);
                            UploadDataManager.Create(uploaddata);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: implement log
                        UploadDataManager.Delete(upload.Id);
                        UploadManager.Delete(upload);
                        throw new HttpException(400, "Upload failed");
                    }
                    return Json(new { code = "00", message = "Successful" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new HttpException(400, "Upload creation failed");
                }

            }
            throw new HttpException(400, "Invalid Data Submitted");
        }

        //
        // POST: /HOOPeration/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Upload(UploadModel model)
        {
            if (ModelState.IsValid)
            {
                List<PDataModel> PData = null;
                try
                {
                    var fileParts = model.PaymentFile.FileName.Split('.');
                    var ext = fileParts[fileParts.Length - 1];
                    var upload_ref_filename = fileParts[0];
                    string savedfilepath = saveUploadedFile(model.PaymentFile);
                    switch (ext)
                    {
                        case "csv":
                            PData = getPaymentDataFromCSV(savedfilepath);
                            break;
                        case "xlsx":
                            PData = getPaymentDataFromXlsx(savedfilepath);
                            break;
                        case "xls":
                            PData = getPaymentDataFromXls(savedfilepath);
                            break;
                        default:
                            throw new HttpException(400, "Unknow file type uploaded");
                    }

                    var bankAccounts = await new AccountNumberStore<IdentityAccountNumber>(new ApplicationDbContext()).GetAllAccountNumbers();

                    foreach (var p in PData)
                    {
                        var pAcct = bankAccounts.Find(k => k.AccountNumber == p.AccountNumber);
                        p.AccountName = pAcct == null ? string.Empty : pAcct.AccountName;
                        p.Id = Guid.NewGuid().ToString();
                        p.AccountNumber = p.AccountNumber == null ? string.Empty : p.AccountNumber;
                    }
                    return Json(new { code = "00", uploadData = PData, accounts = bankAccounts }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw new HttpException(400, ex.Message);
                }
            }
            throw new HttpException(400, "Invalid Data Submitted");
        }

        private List<PDataModel> getPaymentDataFromXls(string filepath)
        {
            List<PDataModel> PData = new List<PDataModel>();
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("Sheet1");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    PData.Add(new PDataModel
                    {
                        Narration = sheet.GetRow(row).GetCell(0).StringCellValue,
                        Amount = Convert.ToDecimal(sheet.GetRow(row).GetCell(1).StringCellValue),
                        AccountNumber = sheet.GetRow(row).GetCell(2).StringCellValue
                    });
                }
            }
            return PData;
        }

        private List<PDataModel> getPaymentDataFromXlsx(string filepath)
        {
            List<PDataModel> PData = new List<PDataModel>();
            XSSFWorkbook xssfwb;
            using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                xssfwb = new XSSFWorkbook(file);
            }

            ISheet sheet = xssfwb.GetSheet("Sheet1");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    PData.Add(new PDataModel
                    {
                        Narration = sheet.GetRow(row).GetCell(0).StringCellValue,
                        Amount = Convert.ToDecimal(sheet.GetRow(row).GetCell(1).StringCellValue),
                        AccountNumber = sheet.GetRow(row).GetCell(2).StringCellValue
                    });
                }
            }
            return PData;
        }
        private string saveUploadedFile(HttpPostedFileBase postedFile)
        {
            string filepath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filepath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filepath);
            }
            return filepath;
        }

        private List<PDataModel> getPaymentDataFromCSV(string filepath)
        {
            List<PDataModel> PData = new List<PDataModel>();
            try
            {
                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filepath);

                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    var split = row.Split(',');
                    if (!string.IsNullOrEmpty(row))
                    {
                        PData.Add(new PDataModel
                        {
                            AccountNumber = row.Split(',')[0],
                            Debit1Credit0 = string.IsNullOrEmpty(row.Split(',')[2]) ? string.IsNullOrEmpty(row.Split(',')[3]) ? false : false : true,
                            PostingCode = row.Split(',')[4],
                            Narration = row.Split(',')[5],
                            Amount = string.IsNullOrEmpty(row.Split(',')[2]) ? string.IsNullOrEmpty(row.Split(',')[3]) ? 0 : Convert.ToDecimal(row.Split(',')[3]) : Convert.ToDecimal(row.Split(',')[2])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: implement log
                throw new Exception("Inavlid File Uploaded");
            }

            return PData;
        }
    }
}