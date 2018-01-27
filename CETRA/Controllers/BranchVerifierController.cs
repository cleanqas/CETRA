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
    public class BranchVerifierController : Controller
    {
        public UploadManager<UploadEntity> UploadManager;
        public UploadDataManager<UploadDataEntity> UploadDataManager;
        public BranchManager<IdentityBranch> BranchManager { get; private set; }

        public BranchVerifierController():
            this(new UploadManager<UploadEntity>(new UploadStore<UploadEntity>(new ApplicationDbContext())),
                new UploadDataManager<UploadDataEntity>(new UploadDataStore<UploadDataEntity>(new ApplicationDbContext())),
                new BranchManager<IdentityBranch>(new BranchStore<IdentityBranch>(new ApplicationDbContext())))
        {
        }

        public BranchVerifierController(UploadManager<UploadEntity> uploadManager,
            UploadDataManager<UploadDataEntity> uploadDataManager,
            BranchManager<IdentityBranch> branchManager)
        {
            UploadManager = uploadManager;
            UploadDataManager = uploadDataManager;
            BranchManager = branchManager;
        }

        // GET: BranchVerifier
        [Authorize]
        public ActionResult Index()
        {
            var userbranch = BranchManager.GetUserBranchByUserId(User.Identity.GetUserId());
            ViewBag.UserBranch = userbranch.Id;
            return View();
        }

        //POST: /BranchVerifier/GetAllPendingVerificationBranchUploads
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllPendingVerificationBranchUploads(string branchId)
        {
            var branchUploads = await new UploadStore<UploadEntity>(new ApplicationDbContext()).FindPendingVerificationUploadsByBranchIdAsync(branchId);
            return Json(new { data = branchUploads }, JsonRequestBehavior.AllowGet);
        }

        //POST: /BranchVerifier/GetAllUploadDataDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllUploadDataDetail(string UploadId)
        {
            var uploadData = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).GetUploadsDataWithAccountName(UploadId);
            return Json(uploadData, JsonRequestBehavior.AllowGet);
        }

        //POST: /BranchVerifier/ApproveUpload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ApproveUpload(string UploadId)
        {
            var uploadStore = new UploadStore<UploadEntity>(new ApplicationDbContext());
            await uploadStore.UpdateUploadStatus(UploadId, 2);
            await uploadStore.UpdateUploadVerifier(UploadId, User.Identity.GetUserId());
            return Json(new { code = "00", message = "Successful" }, JsonRequestBehavior.AllowGet);
        }

        //POST: /BranchVerifier/ApproveUpload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RejectUpload(RejectUpload model)
        {
            if (ModelState.IsValid & model != null)
            {
                var uploadStore = new UploadStore<UploadEntity>(new ApplicationDbContext());
                await uploadStore.UpdateUploadStatus(model.UploadId, -1);
                await uploadStore.UpdateUploadVerifier(model.UploadId, User.Identity.GetUserId());
                await uploadStore.UpdateUploadRejectReason(model.UploadId, model.RejectReason);
                return Json(new { code = "00", message = "Successful" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = "02", message = "Invalid Data Submitted" }, JsonRequestBehavior.AllowGet);
        }
    }
}