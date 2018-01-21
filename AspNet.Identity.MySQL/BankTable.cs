using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BankTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public BankTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Inserts a new Bank in the Bank table
        /// </summary>
        /// <param name="bank">The Bank's detail</param>
        /// <returns></returns>
        public int Insert(IdentityBank bank)
        {
            string commandText = "Insert into Banks (Id, BankName) values (@id, @name)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", bank.Name);
            parameters.Add("@id", bank.Id);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets the IdentityBank given the bank Id
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public IdentityBank GetBankById(string bankId)
        {
            var bankName = GetBankName(bankId);
            IdentityBank bank = null;

            if (bankName != null)
            {
                bank = new IdentityBank(bankName, bankId);
            }

            return bank;
        }

        /// <summary>
        /// Returns a bank name given the branchId
        /// </summary>
        /// <param name="bankId">The Bank Id</param>
        /// <returns>Bank name</returns>
        public string GetBankName(string bankId)
        {
            string commandText = "Select BankName from Banks where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", bankId);
            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Deltes a bank from the Banks table
        /// </summary>
        /// <param name="bankId">The bank Id</param>
        /// <returns></returns>
        public int Delete(string branchId)
        {
            string commandText = "Delete from Bank where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", branchId);

            return _database.Execute(commandText, parameters);
        }

        public List<IdentityBank> GetAllBanks()
        {
            List<IdentityBank> banks = new List<IdentityBank>();
            string commandText = "Select Id, BankName from Banks";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { };

            var result = _database.Query(commandText, parameters);
            if (result != null)
            {
                foreach (var res in result)
                {
                    banks.Add(new IdentityBank()
                    {
                        Id = res["Id"],
                        Name = res["BankName"]
                    });
                }
            }

            return banks;
        }
    }
}
