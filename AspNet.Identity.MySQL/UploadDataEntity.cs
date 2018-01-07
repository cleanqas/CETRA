﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadDataEntity
    {
        public UploadDataEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public UploadDataEntity(string uploadId, string narration, decimal amount) : this()
        {
            UploadId = uploadId;
            Narration = narration;
            Amount = amount;
        }

        public UploadDataEntity(string id, string uploadId, string narration, decimal amount, string accountNumber)
        {
            Id = id;
            UploadId = uploadId;
            Narration = narration;
            Amount = amount;
            AccountNumber = accountNumber;
        }

        /// <summary>
        /// Bank ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Upload Id
        /// </summary>
        public string UploadId { get; set; }

        /// <summary>
        /// Narration
        /// </summary>
        public string Narration { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Account Number
        /// </summary>
        public string AccountNumber { get; set; }
    }
}
