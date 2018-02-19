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
        /// Inserts a upload record in the uploads table
        /// </summary>
        /// <param name="upload">The upload record</param>
        /// <returns></returns>
        public int Insert(UploadEntity upload)
        {
            string commandText = "Insert into uploads (Id, UploaderId, BankId, BranchId, Status) values (@id, @uploaderId, @bankId, @branchId, @status)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", upload.Id);
            parameters.Add("@uploaderId", upload.UploaderId);
            parameters.Add("@bankId", upload.BankId);
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
                upload = new UploadEntity(uploadId, uploaddetail["UploaderId"], uploaddetail["BankId"], uploaddetail["BranchId"], Convert.ToInt32(uploaddetail["Status"]));
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
            string commandText = "Select UploaderId, BankId, BranchId, Status from uploads where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", uploadId);
            Dictionary<string, string> uploaddetail = new Dictionary<string, string>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploaddetail["UploaderId"] = res["UploaderId"];
                uploaddetail["BankId"] = res["BankId"];
                uploaddetail["BranchId"] = res["BranchId"];
                uploaddetail["Status"] = res["Status"];
            }
            return uploaddetail;
        }

        /// <summary>
        /// Deletes an upload from the uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int Delete(string uploadId)
        {
            string commandText = "Delete from uploads where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", uploadId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets all uploads from the uploads table by Branch ID
        /// </summary>
        /// <param name="branchId">The Branch Id</param>
        /// <returns></returns>
        /// 
        public List<UploadEntity> GetUploadsByBranchID(string branchId)
        {
            string commandText = "Select Id, UploaderId, BankId, BranchId, Status, UploadDate from uploads where BranchId = @branchId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@branchId", branchId);
            List<UploadEntity> uploads = new List<UploadEntity>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploads.Add(new UploadEntity()
                {
                    BankId = res["BankId"],
                    BranchId = res["BranchId"],
                    Id = res["Id"],
                    Status = Convert.ToInt32(res["Status"]),
                    UploaderId = res["UploaderId"],
                    UploadDate = Convert.ToDateTime(res["UploadDate"]).ToString("dd-MM-yyyy hh:mm:ss")
                });
            }
            return uploads;
        }

        /// <summary>
        /// Update an upload in the uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int UpdateUploadStatus(string uploadId, int status)
        {
            string commandText = "Update uploads  set status = @Status where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", uploadId);
            parameters.Add("@Status", status);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Update an upload in the uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int UpdateUploadOperator(string uploadId, string operatorId)
        {
            string commandText = "Update uploads set OperatorId = @OperatorId where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploadId);
            parameters.Add("@OperatorId", operatorId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Update an upload in the uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int UpdateUploadVerifier(string uploadId, string verifierId)
        {
            string commandText = "Update uploads set VerifierId = @VerifierId where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploadId);
            parameters.Add("@VerifierId", verifierId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Update an upload in the uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int UpdateUploadHOProcessed(string uploadId, string hoProcessorId)
        {
            string commandText = "Update uploads set HOProcessorId = @HOProcessorId where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploadId);
            parameters.Add("@HOProcessorId", hoProcessorId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Update an upload in the uploads table
        /// </summary>
        /// <param name="uploadId">The Upload Id</param>
        /// <returns></returns>
        public int UpdateUploadRejectReason(string uploadId, string rejectReason)
        {
            string commandText = "Update uploads set RejectReason = @RejectReason where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", uploadId);
            parameters.Add("@RejectReason", rejectReason);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Gets all uploads from the uploads table by Status
        /// </summary>
        /// <param name="status">The upload status</param>
        /// <returns></returns>
        /// 
        public List<UploadEntity> GetUploadsByStatus(int status)
        {
            string commandText = "Select Id, UploaderId, BankId, BranchId, Status, UploadDate from uploads where Status = @status";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@status", status);
            List<UploadEntity> uploads = new List<UploadEntity>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploads.Add(new UploadEntity()
                {
                    BankId = res["BankId"],
                    BranchId = res["BranchId"],
                    Id = res["Id"],
                    Status = Convert.ToInt32(res["Status"]),
                    UploaderId = res["UploaderId"],
                    UploadDate = Convert.ToDateTime(res["UploadDate"]).ToString("dd-MM-yyyy hh:mm:ss")
                });
            }
            return uploads;
        }

        /// <summary>
        /// Gets all uploads from the uploads table by Branch ID
        /// </summary>
        /// <param name="branchId">The Branch Id</param>
        /// <returns></returns>
        /// 
        public List<UploadEntity> GetUploadsByBranchAndStatus(string branchId, int status)
        {
            string commandText = "Select Id, UploaderId, BankId, BranchId, Status, UploadDate, OperatorId from uploads where BranchId = @branchId and Status = @Status";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@branchId", branchId);
            parameters.Add("@Status", status);
            List<UploadEntity> uploads = new List<UploadEntity>();
            var result = _database.Query(commandText, parameters);
            foreach (var res in result)
            {
                uploads.Add(new UploadEntity()
                {
                    BankId = res["BankId"],
                    BranchId = res["BranchId"],
                    Id = res["Id"],
                    Status = Convert.ToInt32(res["Status"]),
                    UploaderId = res["UploaderId"],
                    UploadDate = Convert.ToDateTime(res["UploadDate"]).ToString("dd-MM-yyyy hh:mm:ss"),
                    OperatorId = res["OperatorId"]
                });
            }
            return uploads;
        }
    }
}
