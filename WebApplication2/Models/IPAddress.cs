using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApplication2.Models
{
    public class IPAddress : BaseModel
    {
        public IPAddress()
        {

        }

        [Key]
        public int IPAddressID { get; set; }

        public string Address { get; set; }
        public int Failcount { get; set; }
    }
}