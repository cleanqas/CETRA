using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadDataEntity
    {
        public UploadDataEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public UploadDataEntity(string uploadId, string narration, decimal amount, string accountNumber, bool? debitOrCredit, string postingCode) : this()
        {
            UploadId = uploadId;
            Narration = narration;
            Amount = amount;
            AccountNumber = accountNumber;
            Debit1Credit0 = debitOrCredit;
            PostingCode = postingCode;
        }

        public UploadDataEntity(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Bank ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Upload Id
        /// </summary>
        public string UploadId { get; set; }

        /// <summary>
        /// Narration
        /// </summary>
        public string Narration { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Account Number
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Debit or Credit. Debit = 1 and Credit = 0
        /// </summary>
        public Nullable<bool> Debit1Credit0 { get; set; }

        /// <summary>
        /// Posting Code
        /// </summary>
        public string PostingCode { get; set; }
    }

    public class UploadDataWithBankAndAccountDetails : UploadDataEntity
    {
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string BranchCode { get; set; }
    }
}
