using JackMaiMa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JackMaiMa.ViewModels
{
    public class Order_Detail_Create_ViewModel
    {
        public Order_Detail Order_Detail { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}