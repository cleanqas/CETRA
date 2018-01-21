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

        [Authorize]
        public ActionResult Index()
        {
            return View();
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
                        await UploadManager.DeleteAsync(upload);
                        return Json(new { code = "01", message = ex.Message }, JsonRequestBehavior.AllowGet);
                    }

                    UploadDataEntity uploaddata = null;

                    //upload created, now to extract the data. If data extraction fails, we delete the update record
                    try
                    {
                        foreach (var data in PData)
                        {
                            uploaddata = new UploadDataEntity(upload.Id, data.Narration, data.Amount);
                            await UploadDataManager.CreateAsync(uploaddata);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: implement log
                        await UploadDataManager.DeleteAsync(upload.Id);
                        await UploadManager.DeleteAsync(upload);
                        return Json(new { code = "02", message = "Upload failed" }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = "03", message = "Upload creation failed" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { code = "04", message = "Invalid Data Submitted" }, JsonRequestBehavior.AllowGet);
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