using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class IdentityUserBranch
    {
        
        /// <summary>
        /// Constructor that takes names as argument 
        /// </summary>
        /// <param name="name"></param>
        public IdentityUserBranch(string userId, string branchId)
        {
            UserId = userId;
            BranchID = branchId;
        }
        
        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Branch ID
        /// </summary>
        public string BranchID { get; set; }
        
    }
}
