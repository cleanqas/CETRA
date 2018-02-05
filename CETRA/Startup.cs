using AspNet.Identity.MySQL;
using CETRA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(CETRA.Startup))]
namespace CETRA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var branchManager = new BranchManager<IdentityBranch>(new BranchStore<IdentityBranch>(context));
            var bankManager = new BankManager<IdentityBank>(new BankStore<IdentityBank>(context));

            var user = new ApplicationUser();
            //branchManager.CreateUploadStatus();
            if (!roleManager.RoleExists("Admin"))
            {

                var role = new AspNet.Identity.MySQL.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
                
                user.UserName = "administrator";
                
                string userPWD = "password77$";

                var chkUser = UserManager.Create(user, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("BranchOperator"))
            {
                var role = new AspNet.Identity.MySQL.IdentityRole();
                role.Name = "BranchOperator";
                roleManager.Create(role);                
            }

            if (!roleManager.RoleExists("BranchVerifier"))
            {
                var role = new AspNet.Identity.MySQL.IdentityRole();
                role.Name = "BranchVerifier";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("HeadOfficeOperator"))
            {
                var role = new AspNet.Identity.MySQL.IdentityRole();
                role.Name = "HeadOfficeOperator";
                roleManager.Create(role);
            }

            if (!branchManager.BranchExists("Head Office"))
            {
                user = UserManager.FindByName("administrator");

                var bank = new AspNet.Identity.MySQL.IdentityBank();
                bank.Name = ConfigurationManager.AppSettings["OwnerBankName"];
                bankManager.Create(bank);

                var branch = new AspNet.Identity.MySQL.IdentityBranch();
                branch.Name = "Head Office";
                branch.BankId = bank.Id;
                branch.GLAccount = ConfigurationManager.AppSettings["DefaultGLAccount"];
                branchManager.Create(branch);
                branchManager.AddUserToBranch(user.Id, branch.Id);
                branchManager.CreateUploadStatus();                
            } 
        }
    }
}
