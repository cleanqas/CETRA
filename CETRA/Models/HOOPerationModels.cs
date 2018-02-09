using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CETRA.Models
{
    public class HOOPerationModels
    {
    }

    public class UploadModel
    {
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public HttpPostedFileBase PaymentFile { get; set; }
        public int Status { get; set; }
    }

    public class PDataModel
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public Nullable<bool> Debit1Credit0 { get; set; }
        public string Narration { get; set; }
        public string PostingCode { get; set; }
        public decimal Amount { get; set; }
        public string AccountName { get; set; }
    }

    public class UploadDataModel
    {
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public List<PDataModel> PData { get; set; }
    }    
}