using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication2.Attributes;
using WebApplication2.Resources;

namespace WebApplication2.ViewModels
{
    public class AccountChangePasswordForm
    {
        [Display(Name = "oldpassword", ResourceType = typeof(Resource))]
        [Required( ErrorMessage = "Please enter old password") ]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "newpassword", ResourceType = typeof(Resource))]
        [Required(ErrorMessage = "Please enter new password")]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "confirmnewpassword", ResourceType = typeof(Resource))]
        [Compare("Password")]
        [Required(ErrorMessage = "Please enter confirm new password")]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

}