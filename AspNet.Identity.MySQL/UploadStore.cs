using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadStore<TUpload> where TUpload : UploadEntity
    {
        private UploadTable uploadTable;

        public MySQLDatabase Database { get; private set; }

        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public UploadStore()
        {
            new UploadStore<TUpload>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public UploadStore(MySQLDatabase database)
        {
            Database = database;
            uploadTable = new UploadTable(database);
        }

        public Task CreateAsync(TUpload upload)
        {
            if (upload == null)
            {
                throw new ArgumentNullException("Uploads");
            }

            uploadTable.Insert(upload);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUpload upload)
        {
            if (upload == null)
            {
                throw new ArgumentNullException("Uploads");
            }

            uploadTable.Delete(upload.Id);

            return Task.FromResult<Object>(null);
        }

        public Task<TUpload> FindUploadAsync(string uploadId)
        {
            TUpload result = uploadTable.GetUpload(uploadId) as TUpload;

            return Task.FromResult<TUpload>(result);
        }

        public Task<List<TUpload>> FindUploadsByBranchIdAsync(string branchId)
        {
            List<TUpload> result = uploadTable.GetUploadsByBranchID(branchId) as List<TUpload>;

            return Task.FromResult<List<TUpload>>(result);
        }

        public Task<List<TUpload>> FindUploadsByStatusAsync(int status)
        {
            List<TUpload> result = uploadTable.GetUploadsByStatus(status) as List<TUpload>;

            return Task.FromResult<List<TUpload>>(result);
        }

        public Task<List<TUpload>> FindPendingUploadsByBranchIdAsync(string branchId)
        {
            List<TUpload> result = uploadTable.GetUploadsByBranchAndStatus(branchId, 0) as List<TUpload>;

            return Task.FromResult<List<TUpload>>(result);
        }

        public Task<List<TUpload>> FindPendingVerificationUploadsByBranchIdAsync(string branchId)
        {
            List<TUpload> result = uploadTable.GetUploadsByBranchAndStatus(branchId, 1) as List<TUpload>;

            return Task.FromResult<List<TUpload>>(result);
        }

        public Task<int> UpdateUploadStatus(string uploadId, int status)
        {
            int result = uploadTable.UpdateUploadStatus(uploadId, status);
            return Task.FromResult<int>(result);
        }

        public Task<int> UpdateUploadOperator(string uploadId, string operatorId)
        {
            int result = uploadTable.UpdateUploadOperator(uploadId, operatorId);
            return Task.FromResult<int>(result);
        }

        public Task<int> UpdateUploadVerifier(string uploadId, string verifierId)
        {
            int result = uploadTable.UpdateUploadVerifier(uploadId, verifierId);
            return Task.FromResult<int>(result);
        }

        public Task<int> UpdateUploadHOProcessor(string uploadId, string hoProcessorId)
        {
            int result = uploadTable.UpdateUploadHOProcessed(uploadId, hoProcessorId);
            return Task.FromResult<int>(result);
        }

        public Task<int> UpdateUploadRejectReason(string uploadId, string rejectReason)
        {
            int result = uploadTable.UpdateUploadRejectReason(uploadId, rejectReason);
            return Task.FromResult<int>(result);
        }

    }
}
