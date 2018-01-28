using AspNet.Identity.MySQL;
using CETRA.Models;
using Microsoft.AspNet.Identity;
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
        public async Task<JsonResult> GetAllProcessedUploads(string branchId)
        {
            var branchUploads = await new UploadStore<UploadEntity>(new ApplicationDbContext()).FindUploadsByStatusAsync(2);
            return Json(new { data = branchUploads }, JsonRequestBehavior.AllowGet);
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

        //
        // POST: /HOOPeration/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Upload(UploadModel model)
        {
            if (ModelState.IsValid)
            {
                var upload = new UploadEntity(User.Identity.GetUserId(), model.BranchId, 0);
                var result = await UploadManager.CreateAsync(upload);
                if (result)
                {
                    List<PDataModel> PData = null;
                    try
                    {
                        PData = getPaymentDataFromCSV(model.PaymentFile);
                    }
                    catch (Exception ex)
                    {
                        UploadManager.DeleteAsync(upload);
                        throw new HttpException(400, ex.Message);
                    }

                    UploadDataEntity uploaddata = null;

                    //upload created, now to extract the data. If data extraction fails, we delete the update record
                    try
                    {
                        foreach (var data in PData)
                        {
                            uploaddata = new UploadDataEntity(upload.Id, data.Narration, data.Amount);
                            UploadDataManager.CreateAsync(uploaddata);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: implement log
                        UploadDataManager.DeleteAsync(upload.Id);
                        UploadManager.DeleteAsync(upload);
                        throw new HttpException(400, "Upload failed");
                    }

                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new HttpException(400, "Upload creation failed");
                }
            }
            throw new HttpException(400, "Invalid Data Submitted");
        }

        private List<PDataModel> getPaymentDataFromCSV(HttpPostedFileBase postedFile)
        {
            List<PDataModel> PData = new List<PDataModel>();
            try
            {
                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Read the contents of CSV file.
                    string csvData = System.IO.File.ReadAllText(filePath);

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            PData.Add(new PDataModel
                            {
                                Narration = row.Split(',')[0],
                                Amount = Convert.ToDecimal(row.Split(',')[1])
                            });
                        }
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