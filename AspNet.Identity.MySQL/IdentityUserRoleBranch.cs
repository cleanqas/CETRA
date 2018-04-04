using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class IdentityUserRoleBranch: IdentityUser
    {
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
