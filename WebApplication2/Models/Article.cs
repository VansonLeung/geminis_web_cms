using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models
{
    public class Article : BaseModel
    {
        [Key]
        public int ArticleID { get; set; }

        public string Slug { get; set; }

        public string Name { get; set; }

        public string Lang { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }

        public string Excerpt { get; set; }

        public string Keywords { get; set; }

        public int Version { get; set; }

        public int BaseArticleID { get; set; }

        public bool isRequestingApproval { get; set; }

        public bool isApproved { get; set; }

        public bool isPublished { get; set; }

        public bool isFrozen { get; set; }


        public Article makeNewArticleByCloningContent()
        {
            Article a = new Article();
            a.BaseArticleID = BaseArticleID;
            a.Desc = Desc;
            a.Name = Name;
            a.Slug = Slug;
            a.Keywords = Keywords;
            a.Lang = Lang;
            return a;
        }
    }
}