using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication2.Resources;

namespace WebApplication2.Models.Infrastructure
{
    public class BaseItem : BaseModel
    {
        [Key]
        public int ItemID { get; set; }

        public string GetName(string locale = null)
        {
            if (locale != null)
            {
                if (locale.Equals("en"))
                {
                    return name_en;
                }
                if (locale.Equals("zh"))
                {
                    return name_zh;
                }
                if (locale.Equals("cn"))
                {
                    return name_cn;
                }
            }
            return name_en;
        }

        [Required]
        [Display(Name = "Url", ResourceType = typeof(Resource))]
        public string url { get; set; }

        public string getUrl()
        {
            return "Page/" + url;
        }

        [Required]
        [Display(Name = "name_en", ResourceType = typeof(Resource))]
        public string name_en { get; set; }

        [Required]
        [Display(Name = "name_zh", ResourceType = typeof(Resource))]
        public string name_zh { get; set; }

        [Required]
        [Display(Name = "name_cn", ResourceType = typeof(Resource))]
        public string name_cn { get; set; }

        [Display(Name = "imagePath", ResourceType = typeof(Resource))]
        public string imagePath { get; set; }

        [Display(Name = "isEnabled", ResourceType = typeof(Resource))]
        public bool isEnabled { get; set; }

        [Display(Name = "isContentPage", ResourceType = typeof(Resource))]
        public bool isContentPage { get; set; }

        [Display(Name = "isArticleList", ResourceType = typeof(Resource))]
        public bool isArticleList { get; set; }

        [Display(Name = "isVisibleToVisitorOnly", ResourceType = typeof(Resource))]
        public bool isVisibleToVisitorOnly { get; set; }

        [Display(Name = "isVisibleToMembersOnly", ResourceType = typeof(Resource))]
        public bool isVisibleToMembersOnly { get; set; }

        [Display(Name = "isVisibleToTradingOnly", ResourceType = typeof(Resource))]
        public bool isVisibleToTradingOnly { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Minimum value of order should be \"0\"")]
        [Display(Name = "order", ResourceType = typeof(Resource))]
        public int order { get; set; }

        [Display(Name = "isHeaderMenu", ResourceType = typeof(Resource))]
        public bool isHeaderMenu { get; set; }

        [Display(Name = "isFooterMenu", ResourceType = typeof(Resource))]
        public bool isFooterMenu { get; set; }

        [Display(Name = "isShortcut", ResourceType = typeof(Resource))]
        public bool isShortcut { get; set; }

        [Display(Name = "isJumbotron", ResourceType = typeof(Resource))]
        public bool isJumbotron { get; set; }

        [Display(Name = "isBanner", ResourceType = typeof(Resource))]
        public bool isBanner { get; set; }

        [Display(Name = "remarks", ResourceType = typeof(Resource))]
        public string remarks { get; set; }
    }
}