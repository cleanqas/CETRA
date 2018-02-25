using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BranchManager<TBranch> where TBranch : IdentityBranch
    {
        private BranchStore<TBranch> _store;

        public BranchManager(BranchStore<TBranch> store)
        {
            _store = store;
        }

        public bool BranchExists(string branchName)
        {
            return AsyncHelper.RunSync<bool>(() => _store.BranchExists(branchName));
        }

        public bool BranchCodeExists(string branchCode)
        {
            return AsyncHelper.RunSync<bool>(() => _store.BranchExists(branchCode));
        }

        public void Create(TBranch branch)
        {
            AsyncHelper.RunSync(() => _store.CreateAsync(branch));
        }

        public TBranch FindBranchById(string branchId)
        {
            return AsyncHelper.RunSync<TBranch>(() => _store.FindByIdAsync(branchId));
        }

        public void AddUserToBranch(string userId, string branchId)
        {
            AsyncHelper.RunSync(() => _store.AddUserToBranchAsync(userId, branchId));
        }

        public async Task<bool> CreateAsync(TBranch branch)
        {
            try
            {
                await _store.CreateAsync(branch);
                return true;
            }catch(Exception ex)
            {
                //TODO: Log the exception
                return false;
            }
            
        }

        public TBranch GetUserBranchByUserId(string userId)
        {
           return AsyncHelper.RunSync(() => _store.GetUserBranchByUserId(userId));            
        }

        public void CreateUploadStatus()
        {
            _store.CreateUploadStatus();
        }
    }
}
