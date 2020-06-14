using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace ActivityCenter.Models
{
    public class SafePasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {   
            string stringval = (string)value;
            var regexItem = new Regex("^[a-zA-Z0-9_]*$");

            if(String.IsNullOrWhiteSpace(stringval))
            {
                return new ValidationResult("Password Field cannot be empty!");
            }

            if(stringval.Contains(" "))
            {
                return new ValidationResult("Password cannot contain spaces!");
            }


            // Blackbelt feature
            if(regexItem.IsMatch(stringval))
            {
                return new ValidationResult("Password must be 8 characters long and contain letters, symbols, and numbers!");
            }

            return ValidationResult.Success;
        }

    }
}