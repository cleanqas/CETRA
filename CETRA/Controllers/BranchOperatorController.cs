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
    [ClientErrorHandler]
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
        [Authorize(Roles = "BranchOperator")]
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
            List<UploadEntityModel> allPendingUploadsWithDetails = new List<UploadEntityModel>();
            foreach (var data in branchUploads)
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

        //POST: /BranchOperator/GetBankAccounts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetBankAccounts()
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
                foreach (var data in model)
                {
                    var uploaddata = new UploadDataEntity(data.UploadDataId);
                    uploaddata.AccountNumber = data.AccountNumber;
                    uploaddata.Narration = data.Narration;
                    var updatedata = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).UpdateUploadsDataAsync(uploaddata);
                }
                UploadManager.UpdateUploadStatus(model[0].UploadId, 1);
                UploadManager.UpdateUploadOperator(model[0].UploadId, User.Identity.GetUserId());

                var upload = await new UploadStore<UploadEntity>(new ApplicationDbContext()).FindUploadAsync(model[0].UploadId);
                var branchDetail = await Helper.GetBranchNameAndCode(upload.BranchId);
                new EmailSender().SendToBranchVerifier(branchDetail["BranchCode"]);

                return Json(new { code = "00", message = "Successful" }, JsonRequestBehavior.AllowGet);

            }
            throw new Exception("Invalid Data Submitted");
        }

        //POST: /BranchOperator/GetUploadData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetUploadData(string uploadId)
        {
            var uploadData = await new UploadDataStore<UploadDataEntity>(new ApplicationDbContext()).FindUploadDataAsync(uploadId);

            var bankAccounts = await new AccountNumberStore<IdentityAccountNumber>(new ApplicationDbContext()).GetAllAccountNumbers();
            List<PDataModel> PData = new List<PDataModel>();
            uploadData.ForEach(k => PData.Add(new PDataModel()
            {
                AccountNumber = k.AccountNumber,
                Amount = k.Amount,
                Debit1Credit0 = k.Debit1Credit0,
                Narration = k.Narration,
                PostingCode = k.PostingCode,
                Id = k.Id,
                UploadId = uploadId
            }));

            foreach (var p in PData)
            {
                var pAcct = bankAccounts.Find(k => k.AccountNumber == p.AccountNumber);
                p.AccountName = pAcct == null ? string.Empty : pAcct.AccountName;
                p.AccountNumber = p.AccountNumber == null ? string.Empty : p.AccountNumber;
            }

            return Json(PData, JsonRequestBehavior.AllowGet);
        }

    }
}