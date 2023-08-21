using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class Order_Detail
    {
        public int Id { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPricePerDetail { get; set; }

        public decimal DiscountPerDetail { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}