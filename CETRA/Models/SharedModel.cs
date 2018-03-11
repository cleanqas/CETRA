using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CETRA.Models
{
    public class SharedModel
    {
    }

    public class UploadEntityModel
    {
        public string Id { get; set; }
        public string UploaderId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public int Status { get; set; }
        public string UploadDate { get; set; }
        public string OperatorId { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
    }

    public class UploadDataEntityModel
    {
        public string Id { get; set; }
        public string UploadId { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<bool> Debit1Credit0 { get; set; }
        public string PostingCode { get; set; }
        public string TranDate { get; set; }
        public int Status { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string UploadDate { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string AccountName { get; set; }
        public string TranID { get; set; }
    }
}