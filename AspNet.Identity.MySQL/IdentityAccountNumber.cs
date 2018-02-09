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

        public IdentityAccountNumber(string accountNumber, string accountName) : this()
        {
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        public IdentityAccountNumber(string id, string accountNumber, string accountName)
        {
            Id = id;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        /// <summary>
        /// Bank ID
        /// </summary>
        public string Id { get; set; }

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
