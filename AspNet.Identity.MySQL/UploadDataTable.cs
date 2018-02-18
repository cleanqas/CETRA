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
            string commandText = "Insert into uploadsdata (Id, UploadId, Narration, Amount, AccountNumber, DebitOrCredit, PostingCode) values (@Id, @uploadId, @narration, @amount, @accountNo, @debitcredit, @postingCode)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploaddata.Id);
            parameters.Add("@uploadId", uploaddata.UploadId);
            parameters.Add("@narration", uploaddata.Narration);
            parameters.Add("@amount", uploaddata.Amount);
            parameters.Add("@accountNo", uploaddata.AccountNumber);
            parameters.Add("@debitcredit", uploaddata.Debit1Credit0);
            parameters.Add("@postingCode", uploaddata.PostingCode);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes upload data for upload id from the Uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int Delete(string uploadId)
        {
            string commandText = "Delete from uploadsdata where UploadId = @uploadId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@uploadId", uploadId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets all upload data from the uploadsdata table by Upload ID
        /// </summary>
        /// <param name="uploadId">The Branch Id</param>
        /// <returns></returns>
        /// 
        public List<UploadDataEntity> GetUploadsData(string uploadId)
        {
            string commandText = "Select Id, UploadId, Narration, Amount, AccountNumber, DebitOrCredit, PostingCode from uploadsdata where UploadId = @uploadId";
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
                    Narration = res["Narration"],
                    UploadId = res["UploadId"],
                    PostingCode = res["PostingCode"],
                    Debit1Credit0 = Convert.ToBoolean(res["DebitOrCredit"])
                });
            }
            return uploaddata;
        }

        /// <summary>
        /// Gets all upload data with account name from the uploadsdata table by Upload ID join account name table
        /// </summary>
        /// <param name="uploadId">The Branch Id</param>
        /// <returns></returns>
        /// 
        public List<UploadDataWithBankAndAccountDetails> GetUploadsDataWithAccountName(string uploadId)
        {
            string commandText = "Select u.Id, u.UploadId, u.Narration, u.Amount, u.DebitOrCredit, u.PostingCode, (select BankName from banks where Id = up.BankId) BankName, (select BranchCode from branches where Id = up.BranchId)  BranchCode, u.AccountNumber, a.AccountName from uploadsdata u join uploads up on u.UploadId = up.Id left join accountnumbers a on a.AccountNumber = u.AccountNumber where u.UploadId = @uploadId";
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
                    Narration = res["Narration"],
                    UploadId = res["UploadId"],
                    AccountName = res["AccountName"],
                    BankName = res["BankName"],
                    PostingCode = res["PostingCode"],
                    Debit1Credit0 = Convert.ToBoolean(res["DebitOrCredit"]),
                    BranchCode = res["BranchCode"]
                });
            }
            return uploaddata;
        }

        /// <summary>
        /// Update an uploaded data in the uploadsdata table
        /// </summary>
        /// <param name="uploaddata">The uploaded data</param>
        /// <returns></returns>
        /// 
        public bool UpdateUploadsData(UploadDataEntity uploadData)
        {
            string commandText = "Update uploadsdata set AccountNumber = @accountNo, Narration = @narration where Id = @Id ";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploadData.Id);
            parameters.Add("@accountNo", uploadData.AccountNumber);
            parameters.Add("@narration", uploadData.Narration);

            return _database.Execute(commandText, parameters) > 0;
        }
    }
}
