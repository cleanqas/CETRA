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
        public IdentityBranch(string name, string branchCode) : this()
        {
            Name = name;
          //  GLAccount = glAccount;
            BranchCode = branchCode;
        }

        public IdentityBranch(string id, string name, string branchCode)
        {
            Name = name;
            Id = id;
           // GLAccount = glAccount;
            BranchCode = branchCode;
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
        /// Branch GLAccount
        /// </summary>
        //public string GLAccount { get; set; }

        /// <summary>
        /// Branch BranchCode
        /// </summary>
        public string BranchCode { get; set; }
        
    }
}
