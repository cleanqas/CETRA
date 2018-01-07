using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CETRA.Models
{
    public class HOOPerationModels
    {
    }

    public class UploadModel
    {
        public string BranchId { get; set; }
        public int Status { get; set; }
    }

    public class PDataModel
    {
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
    }
}