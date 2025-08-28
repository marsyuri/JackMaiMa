using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "จำเป็นต้องใส่ชื่อสินค้า")]
        [StringLength(20, ErrorMessage = "ใส่ได้ไม่เกิน 20 ตัว")]
        public string Name { get; set; }

        [StringLength(1024, ErrorMessage = "ใส่ได้ไม่เกิน 1024 ตัว")]
        public string Description { get; set; }

        [Required(ErrorMessage = "จำเป็นต้องเลือก 'หมวดสินค้า'")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Required(ErrorMessage = "จำเป็นต้องเลือก 'ซัพพลายเออร์'")]
        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; }

        [ValidateForUnitCost]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal UnitCost { get; set; }

        [ValidateForUnitPrice]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        [ValidateForNoInStock]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int NumberInStock { get; set; }

        [ValidateForNoInSell]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int NumberInSell { get; set; }

        public bool IsNoImage { get; set; }

        [StringLength(1024, ErrorMessage = "ImageUrl ต้องไม่เกิน 1024 ตัว")]
        public string ImageUrl { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        [Timestamp]
        public byte[] Stamp { get; set; }

        public bool IsDeleted { get; set; }

        //public IEnumerable<Order_Detail> Order_Details { get; set; }
    }
}