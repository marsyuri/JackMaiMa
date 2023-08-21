using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class OrderStatus
    {
        public byte Id { get; set; }
        public string Name { get; set; }

        public static readonly byte Processing = 1;
        public static readonly byte Finished = 2;
        public static readonly byte Canceled = 3;
    }
}