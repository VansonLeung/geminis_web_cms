using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Frontend.Resources;

namespace Frontend.Models
{
    public class BaseModel
    {
        [Display(Name = "created_at", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        public DateTime? created_at { get; set; }

        [Display(Name = "modified_at", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        public DateTime? modified_at { get; set; }

        //public Account created_by { get; set; }

        //public Account modified_by { get; set; }
    }
}