using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class IdentityBranch
    {
        /// <summary>
        /// Default constructor for Branch 
        /// </summary>
        public IdentityBranch()
        {
            Id = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// Constructor that takes names as argument 
        /// </summary>
        /// <param name="name"></param>
        public IdentityBranch(string name, string bankId, string glAccount) : this()
        {
            Name = name;
            BankId = bankId;
            GLAccount = glAccount;
        }

        public IdentityBranch(string name, string id, string bankId, string glAccount)
        {
            Name = name;
            Id = id;
            BankId = bankId;
            GLAccount = glAccount;
        }

        /// <summary>
        /// Branch ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Branch name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Branch BankId
        /// </summary>
        public string BankId { get; set; }

        /// <summary>
        /// Branch GLAccount
        /// </summary>
        public string GLAccount { get; set; }
    }
}
