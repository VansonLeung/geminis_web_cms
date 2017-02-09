using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApplication2.Models
{
    public class User : BaseModel
    {
        public User()
        {

        }

        [Key]
        public int UserID { get; set; }

        public string username { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string password { get; set; }
        public string tel { get; set; }
        public string otp { get; set; }
        
    }
}