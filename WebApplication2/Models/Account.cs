using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WebApplication2.Resources;

namespace WebApplication2.Models
{
    public class Account
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
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "confirmpassword", ResourceType = typeof(Resource))]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Role", ResourceType = typeof(Resource))]
        [Required]
        public string Role { get; set; }
    }
}