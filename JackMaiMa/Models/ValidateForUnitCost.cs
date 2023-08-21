using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class ValidateForUnitCost : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Product product = (Product)validationContext.ObjectInstance;
            if (product.UnitCost >= 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("ต้นทุนต่อหน่วย ห้ามต่ำกว่า 0 บาท");
        }
    }
}