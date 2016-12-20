﻿using System.ComponentModel.DataAnnotations;

namespace WebStorage.UI.Models
{
    public class CustomPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var password = value.ToString();

                var hasUpperCase = false;
                var hasLowerCase = false;
                var numDigits = 0;

                foreach (var c in password)
                {
                    if (char.IsUpper(c))
                        hasUpperCase = true;
                    else if (char.IsLower(c))
                        hasLowerCase = true;
                    else if (char.IsDigit(c))
                        numDigits++;
                }

                if (!hasUpperCase || !hasLowerCase || numDigits < 1)
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}