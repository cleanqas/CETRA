using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class IdentityBankGLAccount
    {
        /// <summary>
        /// Default constructor for Branch 
        /// </summary>
        public IdentityBankGLAccount()
        {
            Id = Guid.NewGuid().ToString();
        }
        

        public IdentityBankGLAccount(string bankId, string glAccount) : this()
        {
            BankId = bankId;
            GLAccount = glAccount;
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
        /// GL Account
        /// </summary>
        public string GLAccount { get; set; }
    }
}
