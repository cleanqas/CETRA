using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UserBranchStore<TUserBranch> where TUserBranch : IdentityUserBranch
    {
        private UserBranchTable userBranchTable;
        public MySQLDatabase Database { get; private set; }

        public UserBranchStore(MySQLDatabase database)
        {
            Database = database;
            userBranchTable = new UserBranchTable(database);
        }
        
    }
}
