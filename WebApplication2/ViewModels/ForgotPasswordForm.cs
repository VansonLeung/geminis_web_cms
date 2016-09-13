using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication2.Attributes;
using WebApplication2.Resources;

namespace WebApplication2.ViewModels
{
    public class ForgotPasswordForm
    {
        [Display(Name = "email", ResourceType = typeof(Resource))]
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string email { get; set; }
    }

}