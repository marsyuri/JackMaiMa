using JackMaiMa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JackMaiMa.ViewModels
{
    public class SupplierDetailViewModel
    {
        public Supplier Supplier { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}