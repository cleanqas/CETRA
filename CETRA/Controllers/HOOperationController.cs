﻿using AspNet.Identity.MySQL;
using CETRA.Models;
using Microsoft.AspNet.Identity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CETRA.Controllers
{
    [ClientErrorHandler]
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

        //POST: /HOOperator/UnidentifiedUploads
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetUnidentifiedUploads()
        {
            var uUploads = await new UploadStore<UploadEntity>(new ApplicationDbContext()).UnidentifiedUploads();
            List<UploadEntityModel> allPendingUploadsWithDetails = new List<UploadEntityModel>();
            foreach (var data in uUploads)
            {
                var branchDetail = await Helper.GetBranchNameAndCode(data.BranchId);
                allPendingUploadsWithDetails.Add(new UploadEntityModel()
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
            return Json(new { data = allPendingUploadsWithDetails }, JsonRequestBehavior.AllowGet);
        }

        //POST: /HOOperator/GetAllUploadDataDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllUploadDataDetail(string UploadId)
        {
            var uploadData = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).GetUploadsDataWithAccountName(UploadId);
            return Json(uploadData, JsonRequestBehavior.AllowGet);
        }

        //POST: /HOOperator/UnindentifiedUploadDataDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UnindentifiedUploadDataDetail(string UploadId)
        {
            var uploadData = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).UnindentifiedDataWithAccountName(UploadId);
            var correspondingUpload = await new UploadStore<UploadEntity>(new ApplicationDbContext()).FindUploadAsync(uploadData.First().UploadId);
            List<UploadDataEntityModel> uploadsWithDetails = new List<UploadDataEntityModel>();
            foreach (var data in uploadData)
            {
                var branchDetail = await Helper.GetBranchNameAndCode(correspondingUpload.BranchId);
                uploadsWithDetails.Add(new UploadDataEntityModel()
                {
                    BankId = correspondingUpload.BankId,
                    BankName = await Helper.GetBankName(correspondingUpload.BankId),
                    BranchId = correspondingUpload.BranchId,
                    BranchName = branchDetail["BranchName"],
                    BranchCode = branchDetail["BranchCode"],
                    Id = data.Id,
                    Status = data.Status,
                    UploadDate = correspondingUpload.UploadDate,
                    AccountNumber = data.AccountNumber,
                    Amount = data.Amount,
                    Debit1Credit0 = data.Debit1Credit0,
                    Narration = data.Narration,
                    PostingCode = data.PostingCode,
                    UploadId = data.UploadId,
                    TranDate = data.TranDate,
                    AccountName = data.AccountName
                });
            }
            return Json(uploadsWithDetails, JsonRequestBehavior.AllowGet);
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

        //
        // POST: /HOOperator/GetBankGLAccounts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetBankGLAccounts(string BankId)
        {
            var glAccounts = await new BankGLAccountStore<IdentityBankGLAccount>(new ApplicationDbContext()).GetBankGLAccounts(BankId);
            return Json(glAccounts, JsonRequestBehavior.AllowGet);
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
                    throw new Exception(ex.Message);
                }
                if (result)
                {
                    try
                    {
                        foreach (var data in model.PData)
                        {
                            var uploaddata = new UploadDataEntity(upload.Id, data.Narration, data.Amount, data.AccountNumber, data.Debit1Credit0, data.PostingCode, data.TranID, data.TranDate, 0);
                            UploadDataManager.Create(uploaddata);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: implement log
                        UploadDataManager.Delete(upload.Id);
                        UploadManager.Delete(upload);
                        throw new Exception("Upload failed");
                    }
                    var branchDetail = await Helper.GetBranchNameAndCode(upload.BranchId);
                    new EmailSender().SendToBranchOperator(branchDetail["BranchCode"]);
                    return Json(new { code = "00", message = "Successful" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Upload creation failed");
                }

            }
            throw new Exception("Invalid Data Submitted");
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
                            PData = getPaymentDataFromCSV(savedfilepath, model.GLAccount);
                            break;
                        case "xlsx":
                            PData = getPaymentDataFromXlsx(savedfilepath);
                            break;
                        case "xls":
                            PData = getPaymentDataFromXls(savedfilepath);
                            break;
                        default:
                            throw new Exception("Unknow file type uploaded");
                    }

                    var bankAccounts = await new AccountNumberStore<IdentityAccountNumber>(new ApplicationDbContext()).GetAllAccountNumbers();
                    var BankDetail = await new BankStore<IdentityBank>(new ApplicationDbContext()).FindByIdAsync(model.BankId);

                    foreach (var p in PData)
                    {
                        var pAcct = bankAccounts.Find(k => k.AccountNumber == p.AccountNumber);
                        p.AccountName = pAcct == null ? string.Empty : pAcct.AccountName;
                        p.Id = Guid.NewGuid().ToString();
                        if(p.Debit1Credit0 == false)
                            p.AccountNumber = p.AccountNumber == null ? string.Empty : p.AccountNumber;
                        p.Narration = BankDetail.Acronym + "::" + p.TranID.Substring(0, 8) + "::" + p.TranDate + "::" + p.Narration;

                        if (p.Debit1Credit0 == null || p.PostingCode == null || !ConfigurationManager.AppSettings["PostingCodes"].Contains(p.PostingCode))
                            throw new Exception("Incomplete contents in the file");
                    }
                    PData.OrderBy(p => p.TranID);
                    return Json(new { code = "00", uploadData = PData, accounts = bankAccounts }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            throw new Exception("Invalid Data Submitted");
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

        private List<PDataModel> getPaymentDataFromCSV(string filepath, string glAccount)
        {
            List<PDataModel> PData = new List<PDataModel>();
            try
            {
                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filepath);
                var tranCount = 0;
                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    tranCount += 1;
                    var split = row.Split(',');
                    if (!string.IsNullOrEmpty(row))
                    {
                        var Debit1Credit0 = string.IsNullOrEmpty(row.Split(',')[1]) ? string.IsNullOrEmpty(row.Split(',')[2]) ? false : false : true;
                        var tranRef = randomdigits();
                        PData.Add(new PDataModel
                        {
                            TranDate = row.Split(',')[0],
                            Amount = string.IsNullOrEmpty(row.Split(',')[1]) ? string.IsNullOrEmpty(row.Split(',')[2]) ? 0 : Convert.ToDecimal(row.Split(',')[2]) : Convert.ToDecimal(row.Split(',')[1]),
                            Debit1Credit0 = Debit1Credit0,
                            PostingCode = "203",
                            Narration = row.Split(',')[3],
                            Status = 0,
                            TranID = tranRef + (!Debit1Credit0 ? "CL" : "GL"),
                            AccountNumber = !Debit1Credit0 ? string.Empty : glAccount
                        });
                        if (!Debit1Credit0)
                            PData.Add(new PDataModel
                            {
                                AccountNumber = glAccount,
                                TranDate = row.Split(',')[0],
                                Amount = Convert.ToDecimal(row.Split(',')[2]),
                                Debit1Credit0 = true,
                                PostingCode = "203",
                                Narration = row.Split(',')[3],
                                Status = 0,
                                TranID = tranRef + "GL"
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

        private string randomdigits()
        {
            int guid = Guid.NewGuid().GetHashCode();
            return guid.ToString().Replace('-', '0').PadLeft(8, '0').Substring(0, 8);
        }
    }
}