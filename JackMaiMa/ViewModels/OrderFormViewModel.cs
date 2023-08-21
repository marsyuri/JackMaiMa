using JackMaiMa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JackMaiMa.ViewModels
{
    public class OrderFormViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
    }
}