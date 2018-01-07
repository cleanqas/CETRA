using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class IdentityAccountNumber
    {
        public IdentityAccountNumber()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityAccountNumber(string bankId, string accountNumber, string accountName) : this()
        {
            BankId = bankId;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        public IdentityAccountNumber(string id, string bankId, string accountNumber, string accountName)
        {
            BankId = bankId;
            Id = id;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        /// <summary>
        /// Bank ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Bank Id
        /// </summary>
        public string BankId { get; set; }

        /// <summary>
        /// AccountNumber
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// AccountName
        /// </summary>
        public string AccountName { get; set; }
    }
}
