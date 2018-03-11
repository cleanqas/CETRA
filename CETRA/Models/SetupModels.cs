using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CETRA.Models
{
    public class SetupModels
    {
    }

    public class BranchModel
    {
        public string Id { get; set; }
        [Required]
        public string BranchName { get; set; }
        [Required]
        public string GLAccount { get; set; }
        [Required]
        public string BranchCode { get; set; }
    }

    public class BankModel
    {
        public string Id { get; set; }
        [Required]
        public string BankName { get; set; }
    }

    public class AccountNumberModel
    {
        public string Id { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string AccountName { get; set; }
        public string AccountBranch { get; set; }
    }

    public class BulkAccountUploadModel
    {
        public HttpPostedFileBase AccountFile { get; set; }
    }

    public class GLAccountModel
    {
        public string Id { get; set; }
        [Required]
        public string BankId { get; set; }
        [Required]
        public string GLAccount { get; set; }
    }
}