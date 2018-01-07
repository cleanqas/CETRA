using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class AccountNumberManager<TAccount> where TAccount : IdentityAccountNumber
    {
        private AccountNumberStore<TAccount> _store;

        public AccountNumberManager(AccountNumberStore<TAccount> store)
        {
            _store = store;
        }

        public void Create(TAccount account)
        {
            AsyncHelper.RunSync(() => _store.CreateAsync(account));
        }

        public TAccount FindAccountNumber(string accountno)
        {
            return AsyncHelper.RunSync<TAccount>(() => _store.FindAccountNumberAsync(accountno));
        }        

        public async Task<bool> CreateAsync(TAccount account)
        {
            try
            {
                await _store.CreateAsync(account);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }

        }

        public async Task<bool> DeleteAsync(TAccount account)
        {
            try
            {
                await _store.DeleteAsync(account);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }
        }
    }
}
