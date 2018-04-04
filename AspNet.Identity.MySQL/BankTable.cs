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
            string commandText = "Insert into banks (Id, BankName, BankAcronym) values (@id, @name, @acronym)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", bank.Name);
            parameters.Add("@id", bank.Id);
            parameters.Add("@acronym", bank.Acronym);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Updates a Bank in the Bank table
        /// </summary>
        /// <param name="bank">The Bank's detail</param>
        /// <returns></returns>
        public int Update(IdentityBank bank)
        {
            string commandText = "Update banks set BankName = @name, BankAcronym = @acronym where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", bank.Name);
            parameters.Add("@id", bank.Id);
            parameters.Add("@acronym", bank.Acronym);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets the IdentityBank given the bank Id
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public IdentityBank GetBankById(string bankId)
        {
            var bankDetail = GetBankName(bankId);
            IdentityBank bank = null;

            if (bankDetail != null)
            {
                bank = new IdentityBank(bankDetail["BankName"], bankDetail["BankAcronym"], bankId);
            }

            return bank;
        }

        /// <summary>
        /// Returns a bank name given the branchId
        /// </summary>
        /// <param name="bankId">The Bank Id</param>
        /// <returns>Bank name</returns>
        public Dictionary<string, string> GetBankName(string bankId)
        {
            string commandText = "Select BankName, BankAcronym from banks where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", bankId);

            Dictionary<string, string> bankdetail = new Dictionary<string, string>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                bankdetail["BankName"] = res["BankName"];
                bankdetail["BankAcronym"] = res["BankAcronym"];
            }
            return bankdetail;
        }

        /// <summary>
        /// Deltes all glAccounts linked to bank from the bankglaccounts table
        /// </summary>
        /// <param name="bankId">The bank Id</param>
        /// <returns></returns>
        public int DeleteBankGlAccounts(string bankId)
        {
            string commandText = "Delete from bankglaccounts where BankId = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", bankId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deltes a bank from the banks table
        /// </summary>
        /// <param name="bankId">The bank Id</param>
        /// <returns></returns>
        public int Delete(string bankId)
        {
            string commandText = "Delete from banks where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", bankId);

            return _database.Execute(commandText, parameters);
        }

        public List<IdentityBank> GetAllBanks()
        {
            List<IdentityBank> banks = new List<IdentityBank>();
            string commandText = "Select Id, BankName, BankAcronym from banks";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { };

            var result = _database.Query(commandText, parameters);
            if (result != null)
            {
                foreach (var res in result)
                {
                    banks.Add(new IdentityBank()
                    {
                        Id = res["Id"],
                        Name = res["BankName"],
                        Acronym = res["BankAcronym"]
                    });
                }
            }

            return banks;
        }
    }
}
