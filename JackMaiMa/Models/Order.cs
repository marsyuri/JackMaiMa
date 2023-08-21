using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int RunNo { get; set; }

        [Required]
        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequiredDate { get; set; }

        public decimal AllTotalPrice { get; set; }

        public decimal DiscountPerOrder { get; set; }

        public decimal NetPrice { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        [Timestamp]
        public byte[] Stamp { get; set; }

        [Required(ErrorMessage = "จำเป็นต้องเลือก 'ลูกค้า'")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public byte OrderStatusId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public IEnumerable<Order_Detail> Order_Details { get; set; }
    }
}