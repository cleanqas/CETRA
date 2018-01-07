using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BankManager<TBank> where TBank : IdentityBank
    {
        private BankStore<TBank> _store;

        public BankManager(BankStore<TBank> store)
        {
            _store = store;
        }

        public void Create(TBank bank)
        {
            AsyncHelper.RunSync(() => _store.CreateAsync(bank));
        }

        public TBank FindBankById(string bankId)
        {
            return AsyncHelper.RunSync<TBank>(() => _store.FindByIdAsync(bankId));
        }

        public async Task<bool> CreateAsync(TBank bank)
        {
            try
            {
                await _store.CreateAsync(bank);
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
