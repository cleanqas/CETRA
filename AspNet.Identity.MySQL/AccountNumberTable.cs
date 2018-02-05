using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class AccountNumberTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public AccountNumberTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Inserts a new Account Number in the accountnumbers table
        /// </summary>
        /// <param name="AccountNumber">The account's detail</param>
        /// <returns></returns>
        public int Insert(IdentityAccountNumber accountno)
        {
            string commandText = "Insert into accountnumbers (Id, BankId, AccountNumber, AccountName) values (@id, @bankId, @accountNumber, @accountname)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", accountno.Id);
            parameters.Add("@bankId", accountno.BankId);
            parameters.Add("@accountNumber", accountno.AccountNumber);
            parameters.Add("@accountName", accountno.AccountName);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets the IdentityBank given the bank Id
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public IdentityAccountNumber GetAccount(string accountNumber)
        {
            var accountdetail = GetAccountDetail(accountNumber);
            IdentityAccountNumber account = null;

            if (accountdetail != null)
            {
                account = new IdentityAccountNumber(accountdetail["Id"], accountdetail["BankId"], accountNumber, accountdetail["AccountName"]);
            }

            return account;
        }

        /// <summary>
        /// Returns a account Id, bankId and name of the account number
        /// </summary>
        /// <param name="accountnumber">The account number</param>
        /// <returns>Bank name</returns>
        public Dictionary<string, string> GetAccountDetail(string accountNumber)
        {
            string commandText = "Select Id, BankId, AccountName from accountnumbers where AccountNumber = @accountno";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@accountno", accountNumber);
            Dictionary<string, string> accountdetail = new Dictionary<string, string>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                accountdetail["Id"] = res["Id"];
                accountdetail["BankId"] = res["BankId"];
                accountdetail["AccountName"] = res["AccountName"];
            }
            return accountdetail;
        }

        /// <summary>
        /// Deltes an account number from the accountnumbers table
        /// </summary>
        /// <param name="accountnumber">The Account Number</param>
        /// <returns></returns>
        public int Delete(string accountNumber)
        {
            string commandText = "Delete from accountnumbers where AccountNumber = @accountno";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@accountno", accountNumber);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns a account Id, bankId and name of the account number
        /// </summary>
        /// <param name="accountnumber">The account number</param>
        /// <returns>Bank name</returns>
        public List<IdentityAccountNumber> GetAllAccount()
        {
            string commandText = "Select Id, BankId, AccountNumber, AccountName from accountnumbers";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<IdentityAccountNumber> accounts = new List<IdentityAccountNumber>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                accounts.Add(new IdentityAccountNumber()
                {
                    AccountName = res["AccountName"],
                    AccountNumber = res["AccountNumber"],
                    BankId = res["BankId"],
                    Id = res["Id"]
                });
            }
            return accounts;
        }
    }
}
