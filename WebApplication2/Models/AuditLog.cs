using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApplication2.Models.Infrastructure;

namespace WebApplication2.Models
{
    public class AuditLog : BaseModel
    {
        [Key]
        public int logID { get; set; }

        public int? accountID { get; set; }
        [ForeignKey("accountID")]
        public virtual Account account { get; set; }

        public int? articleID { get; set; }
        [ForeignKey("articleID")]
        public virtual Article article { get; set; }

        public int? contentPageID { get; set; }
        [ForeignKey("contentPageID")]
        public virtual ContentPage contentPage { get; set; }

        public int? categoryID { get; set; }
        [ForeignKey("categoryID")]
        public virtual Category category { get; set; }

        public string action { get; set; }
    }
}