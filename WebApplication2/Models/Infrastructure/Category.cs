using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApplication2.Resources;

namespace WebApplication2.Models.Infrastructure
{
    public class Category : BaseItem
    {
        [Display(Name = "parentItemID", ResourceType = typeof(Resource))]
        public int? parentItemID { get; set; }

        [ForeignKey("parentItemID")]
        public virtual Category parentItem { get; set; }

        public virtual ICollection<Category> subcategories { get; set; }
        public virtual ICollection<Article> articles { get; set; }
        public virtual ICollection<ContentPage> contentPages { get; set; }
        public virtual ICollection<AuditLog> auditLogs { get; set; }
    }
}