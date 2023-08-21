using JackMaiMa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JackMaiMa.ViewModels
{
    public class OrderDetail_ViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<Order_Detail> Order_Details { get; set; }
    }
}