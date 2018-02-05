using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BranchTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public BranchTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Check a branch exists in the branches table
        /// </summary>
        /// <param name="branchId">The Branch Id</param>
        /// <returns></returns>
        public bool BranchExists(string branchName)
        {
            string commandText = "Select count(*) as count from branches where BranchName = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", branchName);
            Int64 exst = (Int64)_database.QueryValue(commandText, parameters);
            return exst > 0;
        }

        /// <summary>
        /// Deltes a branch from the branches table
        /// </summary>
        /// <param name="branchId">The branch Id</param>
        /// <returns></returns>
        public int Delete(string branchId)
        {
            string commandText = "Delete from branches where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", branchId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new Branch in the branches table
        /// </summary>
        /// <param name="branchName">The branch's name</param>
        /// <returns></returns>
        public int Insert(IdentityBranch branch)
        {
            string commandText = "Insert into branches (Id, BranchName, BankId, GLAccount) values (@id, @name, @bankId, @glAccount)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", branch.Name);
            parameters.Add("@id", branch.Id);
            parameters.Add("@bankId", branch.BankId);
            parameters.Add("@glAccount", branch.GLAccount);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns a branch name, bankid and GLAccount given the branchId
        /// </summary>
        /// <param name="branchId">The branch Id</param>
        /// <returns>branch name</returns>
        public Dictionary<string, string> GetBranchDetail(string branchId)
        {
            string commandText = "Select BranchName, BankId, GLAccount from branches where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", branchId);
            Dictionary<string, string> branchdetail = new Dictionary<string, string>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                branchdetail["BranchName"] = res["BranchName"];
                branchdetail["BankId"] = res["BankId"];
                branchdetail["GLAccount"] = res["GLAccount"];
            }
            return branchdetail;
        }

        /// <summary>
        /// Returns the Branch Id given a branch name
        /// </summary>
        /// <param name="branchName">Brnch's name</param>
        /// <returns>branch's Id</returns>
        public Dictionary<string, string> GetBranchId(string branchName)
        {
            string commandText = "Select Id, BankId, GLAccount from branches where BranchName = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", branchName } };

            Dictionary<string, string> branchdetail = new Dictionary<string, string>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                branchdetail["Id"] = res["Id"];
                branchdetail["BankId"] = res["BankId"];
                branchdetail["GLAccount"] = res["GLAccount"];
            }
            return branchdetail;
        }

        /// <summary>
        /// Gets the IdentityBranch given the branch Id
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IdentityBranch GetBranchById(string branchId)
        {
            var branchDetail = GetBranchDetail(branchId);
            IdentityBranch branch = null;

            if (branchDetail != null)
            {
                branch = new IdentityBranch(branchDetail["BranchName"], branchId, branchDetail["BankId"], branchDetail["GLAccount"]);
            }

            return branch;
        }

        /// <summary>
        /// Gets the IdentityBranch given the branch name
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns></returns>
        public IdentityBranch GetBranchByName(string branchName)
        {
            var branchDetail = GetBranchId(branchName);
            IdentityBranch branch = null;

            if (branchDetail != null)
            {
                branch = new IdentityBranch(branchName, branchDetail["Id"], branchDetail["BankId"], branchDetail["GLAccount"]);
            }

            return branch;
        }

        public List<IdentityBranch> GetAllBranches()
        {
            List<IdentityBranch> branches = new List<IdentityBranch>();
            string commandText = "Select Id, BranchName, BankId, GLAccount from branches";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { };

            var result = _database.Query(commandText, parameters);
            if (result != null)
            {
                foreach (var res in result)
                {
                    branches.Add(new IdentityBranch()
                    {
                        Id = res["Id"],
                        Name = res["BranchName"],
                        BankId = res["BankId"],
                        GLAccount = res["GLAccount"]
                    });
                }
            }

            return branches;
        }

        public int Update(IdentityBranch branch)
        {
            string commandText = "Update branches set BranchName = @name where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", branch.Id);

            return _database.Execute(commandText, parameters);
        }

        public void CreateUploadStatus()
        {
            string commandText = "INSERT INTO uploadstatus (Id, Descr) VALUES ('0', 'Pending'); INSERT INTO uploadstatus (Id, Descr) VALUES ('1', 'Processed at Branch'); INSERT INTO uploadstatus (Id, Descr) VALUES ('2', 'Approved'); INSERT INTO uploadstatus (Id, Descr) VALUES ('3', 'Downloaded'); INSERT INTO uploadstatus (Id, Descr) VALUES ('-1', 'Rejected');";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            _database.Execute(commandText, parameters);
        }
    }
}
