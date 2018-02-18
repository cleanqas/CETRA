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
            string commandText = "Insert into accountnumbers (Id, AccountNumber, AccountName, AccountBranch) values (@id, @accountNumber, @accountname, @accountbranch)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", accountno.Id);
            parameters.Add("@accountNumber", accountno.AccountNumber);
            parameters.Add("@accountName", accountno.AccountName);
            parameters.Add("@accountbranch", accountno.AccountBranch);

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
                account = new IdentityAccountNumber(accountdetail["Id"], accountNumber, accountdetail["AccountName"], accountdetail["AccountBranch"]);
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
            string commandText = "Select Id, AccountName, AccountBranch from accountnumbers where AccountNumber = @accountno";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@accountno", accountNumber);
            Dictionary<string, string> accountdetail = new Dictionary<string, string>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                accountdetail["Id"] = res["Id"];
                accountdetail["AccountName"] = res["AccountName"];
                accountdetail["AccountBranch"] = res["AccountBranch"];
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
            string commandText = "Select Id, AccountNumber, AccountName from accountnumbers";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<IdentityAccountNumber> accounts = new List<IdentityAccountNumber>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                accounts.Add(new IdentityAccountNumber()
                {
                    AccountName = res["AccountName"],
                    AccountNumber = res["AccountNumber"],
                    AccountBranch = res["AccountBranch"],
                    Id = res["Id"]
                });
            }
            return accounts;
        }

        /// <summary>
        /// Returns a account Id, bankId and name of the account number for a bank
        /// </summary>
        /// <param name="accountnumber">The bank Id</param>
        /// <returns>Bank name</returns>
        public List<IdentityAccountNumber> GetBankAccounts(string bankId)
        {
            string commandText = "Select Id, AccountNumber, AccountName from accountnumbers where BankId = @bankId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@bankId", bankId);

            List<IdentityAccountNumber> accounts = new List<IdentityAccountNumber>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                accounts.Add(new IdentityAccountNumber()
                {
                    AccountName = res["AccountName"],
                    AccountNumber = res["AccountNumber"],
                    AccountBranch = res["AccountBranch"],
                    Id = res["Id"]
                });
            }
            return accounts;
        }
    }
}
