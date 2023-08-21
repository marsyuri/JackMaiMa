using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class ValidateForNoInSell : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Product product = (Product)validationContext.ObjectInstance;
            if (product.NumberInSell >= 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("จำนวนที่ขายได้ ห้ามต่ำกว่า 0 หน่วย");
        }
    }
}