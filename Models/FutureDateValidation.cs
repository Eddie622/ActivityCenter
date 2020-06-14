using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models
{
    public class FutureDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value <= DateTime.Now.Date)
                return new ValidationResult("Date must be at least 1 Day in the future");
            return ValidationResult.Success;
        }

    }
}