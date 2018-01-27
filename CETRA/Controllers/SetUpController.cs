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
    public class SetUpController : Controller
    {

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public BranchManager<IdentityBranch> BranchManager { get; private set; }
        public BankManager<IdentityBank> BankManager { get; private set; }
        public AccountNumberManager<IdentityAccountNumber> AccountNumberManager { get; private set; }

        public SetUpController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())),
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())),
                new BranchManager<IdentityBranch>(new BranchStore<IdentityBranch>(new ApplicationDbContext())),
                new BankManager<IdentityBank>(new BankStore<IdentityBank>(new ApplicationDbContext())),
                new AccountNumberManager<IdentityAccountNumber>(new AccountNumberStore<IdentityAccountNumber>(new ApplicationDbContext())))
        {
        }

        public SetUpController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            BranchManager<IdentityBranch> branchManager,
            BankManager<IdentityBank> bankManager,
            AccountNumberManager<IdentityAccountNumber> accountManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            BranchManager = branchManager;
            BankManager = bankManager;
            AccountNumberManager = accountManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /SetUp/RegisterNewUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userexist = UserManager.FindByNameAsync(model.UserName);

                if(userexist != null && userexist.Result != null && userexist.Result.Id != null) return Json(new { code = "02", message = userexist }, JsonRequestBehavior.AllowGet);

                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var role = RoleManager.FindById(model.Role);
                    UserManager.AddToRole(user.Id, role.Name);
                    var branch = BranchManager.FindBranchById(model.Branch);
                    BranchManager.AddUserToBranch(user.Id, branch.Id);
                    return Json(new { code = "00", message = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = "01", message = result }, JsonRequestBehavior.AllowGet);
                }
            }

            // If we got this far, something failed, redisplay form
            return Json(new { code = "02", message = "Invalid Data Submitted" }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SetUp/RegisterNewBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewBranch(BranchModel model)
        {
            if (ModelState.IsValid)
            {
                var branch = new IdentityBranch(model.BranchName, model.BankId, model.GLAccount);
                var result = await BranchManager.CreateAsync(branch);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = "01", message = "Branch creation failed" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { code = "02", message = "Invalid Data Submitted" }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SetUp/RegisterNewBank
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewBank(BankModel model)
        {
            if (ModelState.IsValid)
            {
                var bank = new IdentityBank(model.BankName);
                var result = await BankManager.CreateAsync(bank);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = "01", message = "Bank creation failed" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { code = "02", message = "Invalid Data Submitted" }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SetUp/RegisterNewAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewAccount(AccountNumberModel model)
        {
            if (ModelState.IsValid)
            {
                var account = new IdentityAccountNumber(model.BankId, model.AccountNumber, model.AccountName);
                var result = await AccountNumberManager.CreateAsync(account);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = "01", message = "Bank creation failed" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { code = "02", message = "Invalid Data Submitted" }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SetUp/GetAllBranches
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllBranches()
        {
            var branches = await new BranchStore<IdentityBranch>(new ApplicationDbContext()).GetAllBranchesAsync();
            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SetUp/GetAllBanks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllBanks()
        {
            var banks = await new BankStore<IdentityBank>(new ApplicationDbContext()).GetAllBanksAsync();
            return Json(banks, JsonRequestBehavior.AllowGet);
        }
    }
}