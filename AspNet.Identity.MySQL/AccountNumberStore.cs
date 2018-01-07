using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class AccountNumberStore<TAccountNo> where TAccountNo : IdentityAccountNumber
    {
        private AccountNumberTable accountNoTable;

        public MySQLDatabase Database { get; private set; }

        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public AccountNumberStore()
        {
            new AccountNumberStore<TAccountNo>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public AccountNumberStore(MySQLDatabase database)
        {
            Database = database;
            accountNoTable = new AccountNumberTable(database);
        }

        public Task CreateAsync(TAccountNo accountno)
        {
            if (accountno == null)
            {
                throw new ArgumentNullException("AccountNumber");
            }

            accountNoTable.Insert(accountno);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TAccountNo accountno)
        {
            if (accountno == null)
            {
                throw new ArgumentNullException("AccountNumber");
            }

            accountNoTable.Delete(accountno.Id);

            return Task.FromResult<Object>(null);
        }

        public Task<TAccountNo> FindAccountNumberAsync(string accountno)
        {
            TAccountNo result = accountNoTable.GetAccount(accountno) as TAccountNo;

            return Task.FromResult<TAccountNo>(result);
        }
    }
}
