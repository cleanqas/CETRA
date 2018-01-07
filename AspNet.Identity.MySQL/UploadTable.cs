using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UploadTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Inserts a upload record in the Uploads table
        /// </summary>
        /// <param name="upload">The upload record</param>
        /// <returns></returns>
        public int Insert(UploadEntity upload)
        {
            string commandText = "Insert into Uploads (Id, UploaderId, BranchId, Status) values (@id, @uploaderId, @branchId, @status)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", upload.Id);
            parameters.Add("@uploaderId", upload.UploaderId);
            parameters.Add("@branchId", upload.BranchId);
            parameters.Add("@status", upload.Status);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets the IdentityBank given the bank Id
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public UploadEntity GetUpload(string uploadId)
        {
            var uploaddetail = GetUploadDetail(uploadId);
            UploadEntity upload = null;

            if (uploaddetail != null)
            {
                upload = new UploadEntity(uploadId, uploaddetail["UploaderId"], uploaddetail["BranchId"], Convert.ToInt32(uploaddetail["status"]));
            }

            return upload;
        }

        /// <summary>
        /// Returns a uploader Id, branch Id and status of the uploadId
        /// </summary>
        /// <param name="uploadId">The upload id</param>
        /// <returns>Bank name</returns>
        public Dictionary<string, string> GetUploadDetail(string uploadId)
        {
            string commandText = "Select UploaderId, BranchId, Status from Uploads where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", uploadId);
            Dictionary<string, string> uploaddetail = new Dictionary<string, string>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploaddetail["UploaderId"] = res["UploaderId"];
                uploaddetail["BranchId"] = res["BranchId"];
                uploaddetail["Status"] = res["Status"];
            }
            return uploaddetail;
        }

        /// <summary>
        /// Deletes an upload from the Uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int Delete(string uploadId)
        {
            string commandText = "Delete from Uploads where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", uploadId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets all uploads from the Uploads table by Branch ID
        /// </summary>
        /// <param name="branchId">The Branch Id</param>
        /// <returns></returns>
        /// 
        public List<UploadEntity> GetUploadsByBranchID(string branchId)
        {
            string commandText = "Select Id, UploaderId, BranchId, Status from Uploads where BranchId = @branchId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@branchId", branchId);
            List<UploadEntity> uploads = new List<UploadEntity>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploads.Add(new UploadEntity()
                {
                    BranchId = res["BranchId"],
                    Id = res["Id"],
                    Status = Convert.ToInt32(res["Status"]),
                    UploaderId = res["UploaderId"]
                });
            }
            return uploads;
        }

        /// <summary>
        /// Gets all uploads from the Uploads table by Status
        /// </summary>
        /// <param name="status">The upload status</param>
        /// <returns></returns>
        /// 
        public List<UploadEntity> GetUploadsByStatus(int status)
        {
            string commandText = "Select Id, UploaderId, BranchId, Status from Uploads where Status = @status";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@status", status);
            List<UploadEntity> uploads = new List<UploadEntity>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploads.Add(new UploadEntity()
                {
                    BranchId = res["BranchId"],
                    Id = res["Id"],
                    Status = Convert.ToInt32(res["Status"]),
                    UploaderId = res["UploaderId"]
                });
            }
            return uploads;
        }
    }
}
