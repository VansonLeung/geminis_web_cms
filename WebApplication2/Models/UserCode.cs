using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApplication2.Models
{
    public class UserCode : BaseModel
    {
        public UserCode()
        {

        }

        [Key]
        public int UserCodeID { get; set; }

        public string Email { get; set; }
        public string Key { get; set; }
        public DateTime RegisteredAt { get; set; }
        public bool Completed { get; set; }
    }
}