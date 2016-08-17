using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class AccountGroup
    {
        [Key]
        public int AccountGroupID { get; set; }
        public string Name { get; set; }

        public string AccessibleArticleGroups { get; set; }
        public List<string> AccessibleArticleGroupList { get; set; }

        public string AccessibleContentPages { get; set; }
        public List<string> AccessibleContentPageList { get; set; }
    }
}