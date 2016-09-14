using Foolproof;
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
using WebApplication2.Resources;

namespace WebApplication2.Models
{
    public class BaseArticle : BaseModel
    {
        [Key]
        public int ArticleID { get; set; }

        [Display(Name = "Slug", ResourceType = typeof(Resource))]
        public string Slug { get; set; }

        [Display(Name = "Url", ResourceType = typeof(Resource))]
        public string Url { get; set; }

        [Display(Name = "categoryID", ResourceType = typeof(Resource))]
        public int? categoryID { get; set; }

        [ForeignKey("categoryID")]
        [Display(Name = "category", ResourceType = typeof(Resource))]
        public virtual Category category { get; set; }

        [Display(Name = "BannerImageUrl", ResourceType = typeof(Resource))]
        public string BannerImageUrl { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        public string Name { get; set; }

        [Display(Name = "Lang", ResourceType = typeof(Resource))]
        public string Lang { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Desc", ResourceType = typeof(Resource))]
        public string Desc { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Excerpt", ResourceType = typeof(Resource))]
        public string Excerpt { get; set; }

        [Display(Name = "Keywords", ResourceType = typeof(Resource))]
        public string Keywords { get; set; }

        [Display(Name = "MetaData", ResourceType = typeof(Resource))]
        public string MetaData { get; set; }

        [Display(Name = "MetaKeywords", ResourceType = typeof(Resource))]
        public string MetaKeywords { get; set; }

        [Display(Name = "Version", ResourceType = typeof(Resource))]
        public int Version { get; set; }

        [Display(Name = "BaseArticleID", ResourceType = typeof(Resource))]
        public int BaseArticleID { get; set; }

        [Display(Name = "isRequestingApproval", ResourceType = typeof(Resource))]
        public bool isRequestingApproval { get; set; }

        [Display(Name = "isApproved", ResourceType = typeof(Resource))]
        public bool isApproved { get; set; }

        [Display(Name = "isUnapproved", ResourceType = typeof(Resource))]
        public bool isUnapproved { get; set; }

        [Display(Name = "approvalRemarks", ResourceType = typeof(Resource))]
        public string approvalRemarks { get; set; }

        [Display(Name = "isPublished", ResourceType = typeof(Resource))]
        public bool isPublished { get; set; }

        [Display(Name = "isFrozen", ResourceType = typeof(Resource))]
        public bool isFrozen { get; set; }

        [Display(Name = "dateApproved", ResourceType = typeof(Resource))]
        public DateTime? dateApproved { get; set; }

        [Display(Name = "dateUnapproved", ResourceType = typeof(Resource))]
        public DateTime? dateUnapproved { get; set; }

        [Display(Name = "datePublished", ResourceType = typeof(Resource))]
        public DateTime? datePublished { get; set; }

        [Display(Name = "datePublishStart", ResourceType = typeof(Resource))]
        public DateTime? datePublishStart { get; set; }
        public string getDatePublishStartRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(datePublishStart);
        }

        [Display(Name = "datePublishEnd", ResourceType = typeof(Resource))]
        public DateTime? datePublishEnd { get; set; }
        public string getDatePublishEndRepresentation()
        {
            return DateTimeExtensions.DateTimeToString(datePublishEnd);
        }

        [Display(Name = "createdBy", ResourceType = typeof(Resource))]
        public int? createdBy { get; set; }
        [Display(Name = "approvedBy", ResourceType = typeof(Resource))]
        public int? approvedBy { get; set; }
        [Display(Name = "publishedBy", ResourceType = typeof(Resource))]
        public int? publishedBy { get; set; }

        [Display(Name = "createdByAccount", ResourceType = typeof(Resource))]
        [ForeignKey("createdBy")]
        public virtual Account createdByAccount { get; set; }

        [Display(Name = "approvedByAccount", ResourceType = typeof(Resource))]
        [ForeignKey("approvedBy")]
        public virtual Account approvedByAccount { get; set; }

        [Display(Name = "publishedByAccount", ResourceType = typeof(Resource))]
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