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
        [Authorize(Roles = "BranchVerifier")]
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
            List<UploadEntityModel> allPendingVerificationUploadsWithDetails = new List<UploadEntityModel>();
            foreach (var data in branchUploads)
            {
                var branchDetail = await Helper.GetBranchNameAndCode(data.BranchId);
                allPendingVerificationUploadsWithDetails.Add(new UploadEntityModel()
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
            return Json(new { data = allPendingVerificationUploadsWithDetails }, JsonRequestBehavior.AllowGet);
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
            throw new HttpException(400, "Invalid Data Submitted");
        }
    }
}