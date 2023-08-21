using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JackMaiMa.Models
{
    public class GreaterOrEqual80PercentUnitCost : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Product product = (Product)validationContext.ObjectInstance;
            decimal eightyPercent = 0.8m * product.UnitCost;
            if (product.UnitPrice >= eightyPercent)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(String.Format("ราคาขายต่อหน่วย ห้ามตำกว่า 80% ของต้นทุนต่อหน่วย โดย 80% ของต้นทุนต่อหน่วย = {0} บาท", eightyPercent));
            }
        }
    }
}