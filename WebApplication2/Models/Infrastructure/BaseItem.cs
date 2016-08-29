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

        public string url { get; set; }

        public string name_en { get; set; }
        public string name_zh { get; set; }
        public string name_cn { get; set; }
        public string imagePath { get; set; }

        public bool isClickable { get; set; }
        public bool isEnabled { get; set; }

        public bool isContentPage { get; set; }
        public bool isArticleList { get; set; }

        public bool isVisibleToVisitorOnly { get; set; }
        public bool isVisibleToMembersOnly { get; set; }
        public bool isVisibleToTradingOnly { get; set; }

        public int order { get; set; }
    }
}