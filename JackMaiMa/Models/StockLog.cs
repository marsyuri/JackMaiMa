using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class StockLog
    {
        public int Id { get; set; }

        public int RunningNo { get; set; }

        [Required]
        public string LogNo { get; set; }

        public int OldStock { get; set; }

        public int NumberOfChange { get; set; }

        public int CurrentStock { get; set; }

        public string Remarks { get; set; }

        [Required]
        public string LogBy { get; set; }

        public DateTime LogDate { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public byte StockLogTypeId { get; set; }

        public StockLogType StockLogType { get; set; }
    }
}