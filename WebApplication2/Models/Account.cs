using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WebApplication2.Resources;
using WebApplication2.Attributes;

namespace WebApplication2.Models
{
    public class Account : BaseModel
    {
        [Key]
        public int AccountID { get; set; }

        [Display(Name = "username", ResourceType = typeof(Resource))]
        [Required]
        public string Username { get; set; }

        [Display(Name = "email", ResourceType = typeof(Resource))]
        [Required]
        public string Email { get; set; }

        [Display(Name = "firstname", ResourceType = typeof(Resource))]
        [Required]
        public string Firstname { get; set; }

        [Display(Name = "lastname", ResourceType = typeof(Resource))]
        [Required]
        public string Lastname { get; set; }

        [Display(Name = "password", ResourceType = typeof(Resource))]
        [Required]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "confirmpassword", ResourceType = typeof(Resource))]
        [Compare("Password")]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "role", ResourceType = typeof(Resource))]
        [Required]
        public string Role { get; set; }

        [Display(Name = "lastlogin", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "lastPasswordModifiedAt", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        public DateTime? LastPasswordModifiedAt { get; set; }

        [Display(Name = "historyPasswords", ResourceType = typeof(Resource))]
        public string historyPasswords { get; set; }

        public List<string> historyPasswordList()
        {
            var passwordString = historyPasswords != null ? historyPasswords : "";
            var passwords = passwordString.Split(new char[] { ',' });
            var passwordList = new List<string>(passwords);
            return passwordList;
        }

        public string historyPasswordsFromList(List<string> passwordList)
        {
            var passwords = passwordList.ToArray();
            var passwordString = String.Join(",", passwords);
            return passwordString;
        }
    }

    public class AccountChangePasswordForm
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "oldpassword", ResourceType = typeof(Resource))]
        [Required]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        
        [Display(Name = "newpassword", ResourceType = typeof(Resource))]
        [Required]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "confirmnewpassword", ResourceType = typeof(Resource))]
        [Compare("Password")]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}