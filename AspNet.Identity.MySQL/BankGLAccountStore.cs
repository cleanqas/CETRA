using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BankGLAccountStore<TBankGL> where TBankGL : IdentityBankGLAccount
    {
        private BankGLAccountTable bankGLAccountTable;

        public MySQLDatabase Database { get; private set; }

        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public BankGLAccountStore()
        {
            new BankGLAccountStore<TBankGL>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public BankGLAccountStore(MySQLDatabase database)
        {
            Database = database;
            bankGLAccountTable = new BankGLAccountTable(database);
        }

        public Task<bool> CreateAsync(TBankGL bankGL)
        {
            if (bankGL == null)
            {
                throw new ArgumentNullException("bank account GL");
            }

            bankGLAccountTable.Insert(bankGL);

            return Task.FromResult<bool>(true);
        }

        public Task<List<TBankGL>> GetBankGLAccounts(string bankId)
        {
            if (bankId == null)
            {
                throw new ArgumentNullException("bank account GL");
            }

            var gls = bankGLAccountTable.GetBankGLAccounts(bankId) as List<TBankGL>;

            return Task.FromResult<List<TBankGL>>(gls);
        }
    }
}
