using AspNet.Identity.MySQL;
using CETRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CETRA
{
    public static class Helper
    {
        public static async Task<string> GetBankName(string bankId)
        {
            var bank =  await new BankStore<IdentityBank>(new ApplicationDbContext()).FindByIdAsync(bankId);
            return bank.Name;
        }

        public static async Task<Dictionary<string,string>> GetBranchNameAndCode(string branchId)
        {
            var branch = await new BranchStore<IdentityBranch>(new ApplicationDbContext()).FindByIdAsync(branchId);
            var detail = new Dictionary<string, string>();
            detail.Add("BranchName", branch.Name);
            detail.Add("BranchCode", branch.BranchCode);
            return detail;
        }
    }
}