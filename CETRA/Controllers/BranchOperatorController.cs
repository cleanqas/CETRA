using AspNet.Identity.MySQL;
using CETRA.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CETRA.Controllers
{
    public class BranchOperatorController : Controller
    {
        public UploadManager<UploadEntity> UploadManager;
        public UploadDataManager<UploadDataEntity> UploadDataManager;
        public BranchManager<IdentityBranch> BranchManager { get; private set; }

        public BranchOperatorController() :
            this(new UploadManager<UploadEntity>(new UploadStore<UploadEntity>(new ApplicationDbContext())),
                new UploadDataManager<UploadDataEntity>(new UploadDataStore<UploadDataEntity>(new ApplicationDbContext())),
                new BranchManager<IdentityBranch>(new BranchStore<IdentityBranch>(new ApplicationDbContext())))
        {
        }

        public BranchOperatorController(UploadManager<UploadEntity> uploadManager,
            UploadDataManager<UploadDataEntity> uploadDataManager,
            BranchManager<IdentityBranch> branchManager)
        {
            UploadManager = uploadManager;
            UploadDataManager = uploadDataManager;
            BranchManager = branchManager;
        }

        // GET: BranchOperator
        [Authorize]
        public ActionResult Index()
        {
            var userbranch = BranchManager.GetUserBranchByUserId(User.Identity.GetUserId());
            ViewBag.UserBranch = userbranch.Id;
            return View();
        }

        //POST: /BranchOperator/GetAllPendingBranchUploads
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllPendingBranchUploads(string branchId)
        {
            var branchUploads = await new UploadStore<UploadEntity>(new ApplicationDbContext()).FindPendingUploadsByBranchIdAsync(branchId);
            return Json(new { data = branchUploads }, JsonRequestBehavior.AllowGet);
        }

        //POST: /BranchOperator/GetAllAccounts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllAccounts()
        {
            var accounts = await new AccountNumberStore<IdentityAccountNumber>(new ApplicationDbContext()).GetAllAccountNumbers();
            return Json(accounts, JsonRequestBehavior.AllowGet);
        }

        //POST: /BranchOperator/UpdateUploadData
        [HttpPost]
        public async Task<JsonResult> UpdateUploadData(List<EditUploadData> model)
        {
            if (ModelState.IsValid & model != null)
            {
                foreach(var data in model)
                {
                    var uploaddata = new UploadDataEntity(data.UploadDataId);
                    uploaddata.AccountNumber = data.AccountNumber;
                    uploaddata.BankId = data.BankId;
                    var updatedata = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).UpdateUploadsDataAsync(uploaddata);
                }
                UploadManager.UpdateUploadStatus(model[0].UploadId, 1);
                UploadManager.UpdateUploadOperator(model[0].UploadId, User.Identity.GetUserId());
                return Json(new { code = "00", message = "Successful" }, JsonRequestBehavior.AllowGet);
                
            }
            return Json(new { code = "02", message = "Invalid Data Submitted" }, JsonRequestBehavior.AllowGet);
        }

        //POST: /BranchOperator/GetUploadData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetUploadData(string UploadId)
        {
            var uploadData = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).FindUploadDataAsync(UploadId);
            return Json(uploadData, JsonRequestBehavior.AllowGet);
        }

    }
}