using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Frontend.Attributes
{
    public class AccountPasswordLengthValidationAttribute : ValidationAttribute
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public AccountPasswordLengthValidationAttribute()
            : base(() => "Password must be between 8 - 20 characters")
        {
            this.Minimum = 8;
            this.Maximum = 20;
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                int len = strValue.Length;
                return len >= this.Minimum && len <= this.Maximum;
            }
            return true;
        }
    }
}
