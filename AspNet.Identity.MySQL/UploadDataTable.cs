using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadDataTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UploadDataTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Inserts a upload data record in the UploadData table
        /// </summary>
        /// <param name="uploaddata">The upload data record</param>
        /// <returns></returns>
        public int Insert(UploadDataEntity uploaddata)
        {
            string commandText = "Insert into UploadsData (Id, UploadId, Narration, Amount, AccountNumber) values (@Id, @uploadId, @narration, @amount, @accountNo)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploaddata.Id);
            parameters.Add("@uploadId", uploaddata.UploadId);
            parameters.Add("@narration", uploaddata.Narration);
            parameters.Add("@amount", uploaddata.Amount);
            parameters.Add("@accountNo", uploaddata.AccountNumber);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes upload data for upload id from the Uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int Delete(string uploadId)
        {
            string commandText = "Delete from UploadsData where UploadId = @uploadId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@uploadId", uploadId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets all upload data from the UploadsData table by Upload ID
        /// </summary>
        /// <param name="uploadId">The Branch Id</param>
        /// <returns></returns>
        /// 
        public List<UploadDataEntity> GetUploadsData(string uploadId)
        {
            string commandText = "Select Id, UploadId, Narration, Amount, AccountNumber from Uploadsdata where UploadId = @uploadId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@uploadId", uploadId);
            List<UploadDataEntity> uploaddata = new List<UploadDataEntity>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploaddata.Add(new UploadDataEntity()
                {
                    Id = res["Id"],
                    AccountNumber = res["AccountNumber"],
                    Amount = Convert.ToDecimal(res["Amount"]),
                    Narration = res["Amount"],
                    UploadId = res["UploadId"]
                });
            }
            return uploaddata;
        }

        /// <summary>
        /// Gets all upload data with account name from the UploadsData table by Upload ID join account name table
        /// </summary>
        /// <param name="uploadId">The Branch Id</param>
        /// <returns></returns>
        /// 
        public List<UploadDataWithBankAndAccountDetails> GetUploadsDataWithAccountName(string uploadId)
        {
            string commandText = "Select u.Id, UploadId, Narration, Amount, (select BankName from banks where Id = u.BankId) BankName, u.AccountNumber, a.AccountName from Uploadsdata u left join AccountNumbers a on a.BankId = u.BankId and a.AccountNumber = u.AccountNumber where u.UploadId = @uploadId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@uploadId", uploadId);
            List<UploadDataWithBankAndAccountDetails> uploaddata = new List<UploadDataWithBankAndAccountDetails>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploaddata.Add(new UploadDataWithBankAndAccountDetails()
                {
                    Id = res["Id"],
                    AccountNumber = res["AccountNumber"],
                    Amount = Convert.ToDecimal(res["Amount"]),
                    Narration = res["Amount"],
                    UploadId = res["UploadId"],
                    AccountName = res["AccountName"],
                    BankName = res["BankName"]
                });
            }
            return uploaddata;
        }

        /// <summary>
        /// Update an uploaded data in the UploadsData table
        /// </summary>
        /// <param name="uploaddata">The uploaded data</param>
        /// <returns></returns>
        /// 
        public bool UpdateUploadsData(UploadDataEntity uploadData)
        {
            string commandText = "Update Uploadsdata set AccountNumber = @accountNo, BankId = @bankId where Id = @Id ";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploadData.Id);            
            parameters.Add("@accountNo", uploadData.AccountNumber);
            parameters.Add("@bankId", uploadData.BankId);

            return _database.Execute(commandText, parameters) > 0;
        }
    }
}
