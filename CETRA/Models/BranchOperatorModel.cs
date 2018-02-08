using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CETRA.Models
{
   
    public class EditUploadData
    {
        [Required]
        public string UploadId { get; set; }
        [Required]
        public string UploadDataId { get; set; }
        [Required]
        public string AccountNumber { get; set; }
    }
}