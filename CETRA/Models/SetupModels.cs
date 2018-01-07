using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CETRA.Models
{
    public class SetupModels
    {
    }

    public class BranchModel
    {
        public string Id { get; set; }
        public string BranchName { get; set; }
        public string BankId { get; set; }
        public string GLAccount { get; set; }
    }

    public class BankModel
    {
        public string Id { get; set; }
        public string BankName { get; set; }
    }

    public class AccountNumberModel
    {
        public string Id { get; set; }
        public string BankId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }
}