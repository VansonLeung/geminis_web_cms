using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebApplication2.Resources;
using WebApplication2.Attributes;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.ViewModels
{
    public class AccountLoginForm
    {
        public AccountLoginForm()
        {

        }

        [Key]
        public int AccountID { get; set; }

        [Display(Name = "username", ResourceType = typeof(Resource))]
        [Required]
        public string Username { get; set; }

        [Display(Name = "password", ResourceType = typeof(Resource))]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}