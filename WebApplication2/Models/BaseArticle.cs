using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Helpers;
using WebApplication2.Models.Infrastructure;

namespace WebApplication2.Models
{
    public class BaseArticle : BaseModel
    {
        [Key]
        public int ArticleID { get; set; }
        
        public string Slug { get; set; }

        public string Url { get; set; }

        public int? categoryID { get; set; }
        [ForeignKey("categoryID")]
        public virtual Category category { get; set; }

        public string BannerImageUrl { get; set; }

        public string Name { get; set; }

        public string Lang { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }

        [DataType(DataType.MultilineText)]
        public string Excerpt { get; set; }

        public string Keywords { get; set; }

        public string MetaData { get; set; }

        public string MetaKeywords { get; set; }

        public int Version { get; set; }

        public int BaseArticleID { get; set; }

        public bool isRequestingApproval { get; set; }

        public bool isApproved { get; set; }

        public bool isUnapproved { get; set; }

        public string approvalRemarks { get; set; }

        public bool isPublished { get; set; }

        public bool isFrozen { get; set; }

        public DateTime? dateApproved { get; set; }

        public DateTime? dateUnapproved { get; set; }

        public DateTime? datePublished { get; set; }

        public DateTime? datePublishStart { get; set; }
        public string getDatePublishStartRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(datePublishStart);
        }

        public DateTime? datePublishEnd { get; set; }
        public string getDatePublishEndRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(datePublishEnd);
        }

        public int? createdBy { get; set; }
        public int? approvedBy { get; set; }
        public int? publishedBy { get; set; }
        
        [ForeignKey("createdBy")]
        public virtual Account createdByAccount { get; set; }
        
        [ForeignKey("approvedBy")]
        public virtual Account approvedByAccount { get; set; }
        
        [ForeignKey("publishedBy")]
        public virtual Account publishedByAccount { get; set; }




        public virtual BaseArticle makeNewArticleByCloningContent()
        {
            BaseArticle a = new BaseArticle();
            a.BaseArticleID = BaseArticleID;
            a.categoryID = categoryID;
            a.Excerpt = Excerpt;
            a.Desc = Desc;
            a.Name = Name;
            a.Slug = Slug;
            a.Keywords = Keywords;
            a.Lang = Lang;
            a.createdBy = createdBy;
            a.approvedBy = approvedBy;
            a.publishedBy = publishedBy;
            return a;
        }
    }
}