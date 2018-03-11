using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BankGLAccountTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public BankGLAccountTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Inserts a new Bank GL Account in the BankGLAccounts table
        /// </summary>
        /// <param name="bank">The Bank GL Account detail</param>
        /// <returns></returns>
        public int Insert(IdentityBankGLAccount bankGL)
        {
            string commandText = "Insert into bankglaccounts (Id, BankId, GLAccount) values (@id, @bankId, @glAccount)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@bankId", bankGL.BankId);
            parameters.Add("@id", bankGL.Id);
            parameters.Add("@glAccount", bankGL.GLAccount);

            return _database.Execute(commandText, parameters);
        }

        public List<IdentityBankGLAccount> GetBankGLAccounts(string bankId)
        {
            string commandText = "Select Id, BankId, GLAccount from bankglaccounts where BankId = @bankid";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@bankid", bankId);
            List<IdentityBankGLAccount> bankGlAccounts = new List<IdentityBankGLAccount>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                bankGlAccounts.Add(new IdentityBankGLAccount()
                {
                    Id = res["Id"],
                    BankId = res["BankId"],
                    GLAccount = res["GLAccount"]
                });
            }
            return bankGlAccounts;
        }
    }
}
