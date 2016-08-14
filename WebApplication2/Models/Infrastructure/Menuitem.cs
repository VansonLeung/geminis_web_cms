using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.Infrastructure
{
    public class Menuitem : BaseItem
    {
        public string url { get; set; }

        public int? parentItemID { get; set; }

        [ForeignKey("parentItemID")]
        public virtual Menuitem parentItem { get; set; }
    }
}