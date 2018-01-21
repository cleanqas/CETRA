using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UserBranchTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UserBranchTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a list of user's branches
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public IdentityBranch FindByUserId(string userId)
        {
            IdentityBranch branch = new IdentityBranch();
            string commandText = "Select Branches.Id, Branches.BranchName from userbranches, Branches where userbranches.UserId = @userId and userbranches.BranchId = Branches.Id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            var rows = _database.Query(commandText, parameters);
            foreach(var row in rows)
            {
                branch.Id = row["Id"];
                branch.Name =  row["BranchName"];
            }

            return branch;
        }

        /// <summary>
        /// Deletes all branches from a user in the UserBranches table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "Delete from userbranches where UserId = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes specific branch from a user in the UserBranches table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId, string branchId)
        {
            string commandText = "Delete from userbranches where UserId = @userId and BranchId = @branchId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserId", userId);
            parameters.Add("BranchId", branchId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new role for a user in the UserRoles table
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="branchId">The Branch's id</param>
        /// <returns></returns>
        public int Insert(string userId, string branchId)
        {
            string commandText = "Insert into userbranches (UserId, BranchId) values (@userId, @branchId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);
            parameters.Add("branchId", branchId);

            return _database.Execute(commandText, parameters);
        }
    }
}
