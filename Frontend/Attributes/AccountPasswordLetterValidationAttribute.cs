using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Frontend.Attributes
{
    public class AccountPasswordLetterValidationAttribute : ValidationAttribute
    {
        public AccountPasswordLetterValidationAttribute()
        : base(() => "Password must have at least one alphabet and one digit")
        {

        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                var hasAlphabetsAndNumbers = Regex.IsMatch(strValue, @"^(?=.*[a-zA-Z])(?=.*[0-9])");

                if (hasAlphabetsAndNumbers)
                {
                    return true;
                }

                return false;
            }
            return true;
        }
    }
}
