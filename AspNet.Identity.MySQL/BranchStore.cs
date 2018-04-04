using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BranchStore<TBranch> where TBranch : IdentityBranch
    {
        private BranchTable branchTable;
        private UserBranchTable userBranchTable;
        public MySQLDatabase Database { get; private set; }

        public IQueryable<TBranch> Branches
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public BranchStore()
        {
            new BranchStore<TBranch>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public BranchStore(MySQLDatabase database)
        {
            Database = database;
            branchTable = new BranchTable(database);
            userBranchTable = new UserBranchTable(database);
        }

        public Task<bool> BranchExists(string branchName)
        {
            if (branchName == null)
            {
                throw new ArgumentNullException("branch");
            }

            return Task.FromResult<bool>(branchTable.BranchExists(branchName));
        }

        public Task<bool> BranchCodeExists(string branchCode)
        {
            if (branchCode == null)
            {
                throw new ArgumentNullException("branch");
            }

            return Task.FromResult<bool>(branchTable.BranchCodeExists(branchCode));
        }

        public Task CreateAsync(TBranch branch)
        {
            if (branch == null)
            {
                throw new ArgumentNullException("branch");
            }

            branchTable.Insert(branch);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TBranch branch)
        {
            if (branch == null)
            {
                throw new ArgumentNullException("branch");
            }
            branchTable.DeleteBranchUploads(branch.Id);
            branchTable.DeleteBranchUsers(branch.Id);
            branchTable.Delete(branch.Id);

            return Task.FromResult<Object>(null);
        }

        public Task<TBranch> FindByIdAsync(string branchId)
        {
            TBranch result = branchTable.GetBranchById(branchId) as TBranch;

            return Task.FromResult<TBranch>(result);
        }

        public Task<TBranch> FindByNameAsync(string branchName)
        {
            TBranch result = branchTable.GetBranchByName(branchName) as TBranch;

            return Task.FromResult<TBranch>(result);
        }

        public Task<List<TBranch>> GetAllBranchesAsync()
        {
            List<TBranch> result = branchTable.GetAllBranches() as List<TBranch>;

            return Task.FromResult<List<TBranch>>(result);
        }

        public Task UpdateAsync(TBranch branch)
        {
            if (branch == null)
            {
                throw new ArgumentNullException("branch");
            }

            branchTable.Update(branch);

            return Task.FromResult<Object>(null);
        }

        public Task AddUserToBranchAsync(string userId, string branchId)
        {
            if (branchId == null | userId == null)
            {
                throw new ArgumentNullException("branch");
            }

            userBranchTable.Insert(userId, branchId);

            return Task.FromResult<object>(null);
        }

        public Task<TBranch> GetUserBranchByUserId(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("branch");
            }

            TBranch userbranch = userBranchTable.FindByUserId(userId) as TBranch;

            return Task.FromResult<TBranch>(userbranch);
        }

        public void CreateUploadStatus()
        {            
            branchTable.CreateUploadStatus();
        }

        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }
    }
}
