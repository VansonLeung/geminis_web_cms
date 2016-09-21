using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using WebApplication2.Helpers;
using WebApplication2.Resources;

namespace WebApplication2.Models
{
    public class SystemMaintenanceNotification : BaseModel
    {
        [Key]
        public int NotificationID { get; set; }

        [Display( Name = "name_en", ResourceType = typeof(Resource))]
        public string name_en { get; set; }

        [Display(Name = "name_zh", ResourceType = typeof(Resource))]
        public string name_zh { get; set; }

        [Display(Name = "name_cn", ResourceType = typeof(Resource))]
        public string name_cn { get; set; }

        [Display(Name = "desc_en", ResourceType = typeof(Resource))]
        public string desc_en { get; set; }

        [Display(Name = "desc_zh", ResourceType = typeof(Resource))]
        public string desc_zh { get; set; }

        [Display(Name = "desc_cn", ResourceType = typeof(Resource))]
        public string desc_cn { get; set; }

        [Display(Name = "isActive", ResourceType = typeof(Resource))]
        public bool isActive { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "startDate", ResourceType = typeof(Resource))]
        public DateTime? startDate { get; set; }
        public string getStartDateRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(startDate);
        }

        [DataType(DataType.DateTime)]
        [Display(Name = "endDate", ResourceType = typeof(Resource))]
        public DateTime? endDate { get; set; }
        public string getEndDateRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(endDate);
        }
    }
}