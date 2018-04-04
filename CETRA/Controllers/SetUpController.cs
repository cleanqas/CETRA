using AspNet.Identity.MySQL;
using CETRA.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CETRA.Controllers
{
    [ClientErrorHandler]
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

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /SetUp/RegisterNewBankGLAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewBankGLAccount(GLAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var glAccount = new IdentityBankGLAccount(model.BankId, model.GLAccount);
                var result = await new BankGLAccountStore<IdentityBankGLAccount>(new ApplicationDbContext()).CreateAsync(glAccount);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("GL Account creation failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }

        #region Accounts

        //
        // POST: /SetUp/GetAllAccounts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllAccounts()
        {
            var accounts = await new AccountNumberStore<IdentityAccountNumber>(new ApplicationDbContext()).GetAllAccountNumbers();
            return Json(accounts, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SetUp/RegisterNewAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewAccount(AccountNumberModel model)
        {
            if (ModelState.IsValid)
            {
                var account = new IdentityAccountNumber(model.AccountNumber, model.AccountName, model.AccountBranch);
                var result = await AccountNumberManager.CreateAsync(account);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("New Account creation failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }

        //
        // POST: /SetUp/BulkRegisterAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> BulkRegisterAccount(BulkAccountUploadModel model)
        {
            string filepath = saveUploadedFile(model.AccountFile);
            try
            {
                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filepath);

                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var split = row.Split(',');
                        var account = new IdentityAccountNumber(split[0], split[1], split[2]);
                        var result = await AccountNumberManager.CreateAsync(account);
                    }
                }
                return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //TODO: implement log
                throw new Exception("Inavlid File Uploaded");
            }
        }

        #endregion

        #region User

        //
        // POST: /SetUp/GetAllUsers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllUsers()
        {
            var users = await new UserStore<IdentityUser>(new ApplicationDbContext()).GetAllUsersAsync();
            return Json(users, JsonRequestBehavior.AllowGet);
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

                if (userexist != null && userexist.Result != null && userexist.Result.Id != null) return Json(new { code = "02", message = userexist }, JsonRequestBehavior.AllowGet);

                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };
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
                    throw new Exception("Cannot Create User");
                }
            }

            throw new Exception("Invalid Data");
        }

        //
        // POST: /SetUp/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Username, Email = model.Email, Id = model.Id };
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {                    
                    return Json(new { code = "00", message = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Cannot Delete User");
                }
            }

            throw new Exception("Invalid Data");
        }

        //
        // POST: /SetUp/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateUser(RegisterViewModel model, string Id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser() { Email = model.Email, Id = Id };
                    await UserManager.AddPasswordAsync(Id, model.Password);
                    await new UserStore<IdentityUser>(new ApplicationDbContext()).DeleteUserRoleAsync(user);
                    UserManager.AddToRole(Id, model.Role);
                    await new UserStore<IdentityUser>(new ApplicationDbContext()).DeleteUserBranchAsync(user);
                    await new BranchStore<IdentityBranch>(new ApplicationDbContext()).AddUserToBranchAsync(Id, model.Branch);
                    return Json(new { code = "00", message = "Successdul" }, JsonRequestBehavior.AllowGet);
                }catch(Exception ex)
                {
                    throw new Exception("Cannot Delete User: " + ex.Message);
                }
            }

            throw new Exception("Invalid Data");
        }
        #endregion

        #region Branches

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
        // POST: /SetUp/RegisterNewBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewBranch(BranchModel model)
        {
            if (ModelState.IsValid)
            {
                var branchExist = BranchManager.BranchExists(model.BranchName);
                if (branchExist) throw new Exception("Branch already exist");

                var branchCodeExist = BranchManager.BranchExists(model.BranchCode);
                if (branchCodeExist) throw new Exception("Branch Code already exist");

                var branch = new IdentityBranch(model.BranchName, model.BranchCode);
                var result = await BranchManager.CreateAsync(branch);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Branch creation failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }

        //
        // POST: /SetUp/EditExistingBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditExistingBranch(BranchModel model)
        {
            if (ModelState.IsValid)
            {                
                var branch = new IdentityBranch(model.Id, model.BranchName, model.BranchCode);
                var result = await BranchManager.UpdateAsync(branch);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Branch update failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }

        //
        // POST: /SetUp/DeleteBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteBranch(BranchModel model)
        {
            if (ModelState.IsValid)
            {
                var branch = new IdentityBranch(model.Id, model.BranchName, model.BranchCode);
                var result = await BranchManager.DeleteAsync(branch);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Branch delete failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }
        #endregion

        #region Bank
        //
        // POST: /SetUp/GetAllBanks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllBanks()
        {
            var banks = await new BankStore<IdentityBank>(new ApplicationDbContext()).GetAllBanksAsync();
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SetUp/RegisterNewBank
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterNewBank(BankModel model)
        {
            if (ModelState.IsValid)
            {
                var bank = new IdentityBank(model.BankName, model.BankAcronym);
                var result = await BankManager.CreateAsync(bank);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Bank creation failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }

        //
        // POST: /SetUp/EditExistingBank
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditExistingBank(BankModel model)
        {
            if (ModelState.IsValid)
            {
                var bank = new IdentityBank(model.BankName, model.BankAcronym, model.Id);
                var result = await BankManager.UpdateAsync(bank);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Bank update failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }

        //
        // POST: /SetUp/DeleteBank
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteBank(BankModel model)
        {
            if (ModelState.IsValid)
            {
                var bank = new IdentityBank(model.BankName, model.BankAcronym, model.Id);
                var result = await BankManager.DeleteAsync(bank);
                if (result)
                {
                    return Json(new { code = "00", message = "Sucessfull" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Bank Delete failed");
                }
            }
            throw new Exception("Invalid Data Submitted");
        }

        #endregion
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
    }
}