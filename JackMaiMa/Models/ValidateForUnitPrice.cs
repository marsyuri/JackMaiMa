using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class ValidateForUnitPrice : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Product product = (Product)validationContext.ObjectInstance;
            decimal eightyPercent = 0.8m * product.UnitCost;
            if (product.UnitPrice < 0)
            {
                return new ValidationResult("ราคาขายต่อหน่วย ห้ามต่ำกว่า 0 บาท");
            }
            else if (product.UnitPrice < eightyPercent)
            {
                return new ValidationResult(String.Format("ราคาขายต่อหน่วย ห้ามตำกว่า 80% ของต้นทุนต่อหน่วย โดย 80% ของต้นทุนต่อหน่วย = {0:n2} บาท", eightyPercent));
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}