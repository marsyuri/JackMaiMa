using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JackMaiMa.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "จำเป็นต้องใส่ชื่อลูกค้า")]
        [StringLength(100, ErrorMessage = "ใส่ได้ไม่เกิน 100 ตัว")]
        public string Name { get; set; }

        [Required(ErrorMessage = "จำเป็นต้องใส่ชื่อผู้ติดต่อ")]
        [StringLength(100, ErrorMessage = "ใส่ได้ไม่เกิน 100 ตัว")]
        public string ContactName { get; set; }

        [StringLength(100, ErrorMessage = "ใส่ได้ไม่เกิน 100 ตัว")]
        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string Tambon { get; set; }

        public string Amphur { get; set; }

        public string Province { get; set; }

        [StringLength(5, ErrorMessage = "ใส่ได้ไม่เกิน 5 หลัก")]
        public string PostalCode { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        [StringLength(512, ErrorMessage = "ใส่ได้ไม่เกิน 512 ตัว")]
        public string OptionalContact { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        [Timestamp]
        public byte[] Stamp { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}