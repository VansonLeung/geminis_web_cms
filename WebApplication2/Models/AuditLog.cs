using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApplication2.Models.Infrastructure;
using WebApplication2.Resources;

namespace WebApplication2.Models
{
    public class AuditLog : BaseModel
    {
        [Key]
        public int logID { get; set; }

        public int? accountID { get; set; }
        public string account { get; set; }

        public int? articleID { get; set; }
        public string article { get; set; }

        public int? contentPageID { get; set; }
        public string contentPage { get; set; }

        public int? categoryID { get; set; }
        public string category { get; set; }

        public int? targetAccountID { get; set; }
        public string targetAccount { get; set; }

        public int? systemMaintenanceNotificationID { get; set; }
        public string systemMaintenanceNotification { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resource))]
        public string action { get; set; }
    }
}