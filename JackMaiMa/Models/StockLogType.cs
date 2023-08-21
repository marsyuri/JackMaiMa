using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class StockLogType
    {
        public byte Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public static readonly byte Edit = 1;
        public static readonly byte Sell = 2;
        public static readonly byte CancelOrderDetail = 3;
    }
}