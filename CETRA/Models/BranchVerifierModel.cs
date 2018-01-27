using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CETRA.Models
{
    public class BranchVerifierModel
    {
    }

    public class RejectUpload
    {
        public string UploadId { get; set; }
        public string UploaderId { get; set; }
        public string OperatorId { get; set; }
        public string RejectReason { get; set; }
    }
}