using JackMaiMa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JackMaiMa.ViewModels
{
    public class CustomerDetailViewModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}