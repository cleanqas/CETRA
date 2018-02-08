using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadEntity
    {
        public UploadEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public UploadEntity(string uploaderId, string bankId, string branchId, int status) : this()
        {
            UploaderId = uploaderId;
            BankId = bankId;
            BranchId = branchId;
            Status = status;
        }

        public UploadEntity(string id, string uploaderId, string bankId, string branchId, int status)
        {
            Id = id;
            UploaderId = uploaderId;
            BankId = bankId;
            BranchId = branchId;
            Status = status;
        }

        /// <summary>
        /// Bank ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Uploader Id
        /// </summary>
        public string UploaderId { get; set; }

        /// <summary>
        /// Bank ID
        /// </summary>
        public string BankId { get; set; }

        /// <summary>
        /// Branch ID
        /// </summary>
        public string BranchId { get; set; }


        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; }

        public string UploadDate { get; set; }

        public string OperatorId { get; set; }
    }
}
