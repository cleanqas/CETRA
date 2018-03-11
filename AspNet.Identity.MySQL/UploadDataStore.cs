using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadDataStore<TUploadData> where TUploadData : UploadDataEntity
    {
        private UploadDataTable uploadDataTable;

        public MySQLDatabase Database { get; private set; }

        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public UploadDataStore()
        {
            new UploadDataStore<TUploadData>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public UploadDataStore(MySQLDatabase database)
        {
            Database = database;
            uploadDataTable = new UploadDataTable(database);
        }

        public Task CreateAsync(TUploadData upload)
        {
            if (upload == null)
            {
                throw new ArgumentNullException("Upload Data");
            }

            uploadDataTable.Insert(upload);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(string uploadId)
        {
            if (uploadId == null)
            {
                throw new ArgumentNullException("Upload Data");
            }

            uploadDataTable.Delete(uploadId);

            return Task.FromResult<Object>(null);
        }

        public Task<List<TUploadData>> FindUploadDataAsync(string uploadId)
        {
            List<TUploadData> result = uploadDataTable.GetUploadsData(uploadId) as List<TUploadData>;

            return Task.FromResult<List<TUploadData>>(result);
        }

        public Task<bool> UpdateUploadsDataAsync(UploadDataEntity uploadData)
        {
            bool result = uploadDataTable.UpdateUploadsDataAccount(uploadData);
            result = uploadDataTable.UpdateUploadsData(uploadData);           

            return Task.FromResult<bool>(result);
        }

        public Task<List<UploadDataWithBankAndAccountDetails>> GetUploadsDataWithAccountName(string uploadId)
        {
            List<UploadDataWithBankAndAccountDetails> result = uploadDataTable.GetUploadsDataWithAccountName(uploadId, true);
            return Task.FromResult<List<UploadDataWithBankAndAccountDetails>>(result);
        }

        public Task<List<UploadDataWithBankAndAccountDetails>> UnindentifiedDataWithAccountName(string uploadId)
        {
            List<UploadDataWithBankAndAccountDetails> result = uploadDataTable.GetUploadsDataWithAccountName(uploadId, false);
            return Task.FromResult<List<UploadDataWithBankAndAccountDetails>>(result);
        }
    }
}
