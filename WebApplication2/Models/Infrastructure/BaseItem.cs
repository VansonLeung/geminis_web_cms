using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.Infrastructure
{
    public class BaseItem : BaseModel
    {
        [Key]
        public int ItemID { get; set; }

        public string GetName(string locale = null)
        {
            return name_en;
        }

        [Required]
        public string url { get; set; }

        public string getUrl()
        {
            return "/" + url;
        }

        [Required]
        public string name_en { get; set; }

        [Required]
        public string name_zh { get; set; }

        [Required]
        public string name_cn { get; set; }

        public string imagePath { get; set; }

        public bool isEnabled { get; set; }

        public bool isContentPage { get; set; }
        public bool isArticleList { get; set; }

        public bool isVisibleToVisitorOnly { get; set; }
        public bool isVisibleToMembersOnly { get; set; }
        public bool isVisibleToTradingOnly { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Minimum value of order should be \"0\"")]
        public int order { get; set; }
    }
}