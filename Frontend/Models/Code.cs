using Frontend.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Frontend.Attributes;
using Frontend.Models;

namespace Frontend.Models
{
    public class Code : BaseModel
    {
        public Code()
        {

        }

        [Key]
        public int CodeID { get; set; }

        public string Email { get; set; }
        public string Key { get; set; }
        public DateTime RegisteredAt { get; set; }
        public bool Completed { get; set; }
    }
}