using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Constant
    {
        [Key]
        public int ConstantID { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public bool isActive { get; set; }
    }
}