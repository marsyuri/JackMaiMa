using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="จำเป็นต้องใส่ชื่อหมวดสินค้า")]
        [StringLength(20, ErrorMessage = "ใส่ได้ไม่เกิน 20 ตัว")]
        public string Name { get; set; }

        [StringLength(1024, ErrorMessage = "ใส่ได้ไม่เกิน 1024 ตัว")]
        public string Description { get; set; }

        public bool IsNoImage { get; set; }

        [StringLength(1024, ErrorMessage = "ImageUrl ต้องไม่เกิน 1024 ตัว")]
        public string ImageUrl { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        [Timestamp]
        public byte[] Stamp { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}