using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class ValidateForNoInStock : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Product product = (Product)validationContext.ObjectInstance;
            if (product.NumberInStock >= 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("จำนวนของใน Stock ห้ามต่ำกว่า 0 หน่วย");
        }
    }
}