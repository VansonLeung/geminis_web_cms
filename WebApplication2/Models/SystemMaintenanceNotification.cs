using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using WebApplication2.Helpers;

namespace WebApplication2.Models
{
    public class SystemMaintenanceNotification : BaseModel
    {
        [Key]
        public int NotificationID { get; set; }

        public string name_en { get; set; }

        public string name_zh { get; set; }

        public string name_cn { get; set; }

        public string desc_en { get; set; }

        public string desc_zh { get; set; }

        public string desc_cn { get; set; }

        public bool isActive { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? startDate { get; set; }
        public string getStartDateRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(startDate);
        }

        [DataType(DataType.DateTime)]
        public DateTime? endDate { get; set; }
        public string getEndDateRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(endDate);
        }
    }
}