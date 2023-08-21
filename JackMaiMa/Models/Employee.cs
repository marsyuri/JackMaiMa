using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public DateTime Birthdate { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }

        public bool IsMarried { get; set; }

        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public string Address { get; set; }

        public string Tambon { get; set; }

        public string Amphur { get; set; }

        public string Province { get; set; }

        [StringLength(5)]
        public string PostalCode { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        [StringLength(512)]
        public string OptionalContact { get; set; }
    }
}