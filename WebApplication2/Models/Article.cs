using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models
{
    public class Article : BaseArticle
    {
        public string getUrl()
        {
            if (category != null)
            {
                var url = category.getUrl();
                var suf = Slug;

                if (url != null
                    && suf != null)
                {
                    return String.Format("{0}/{1}", url, suf);
                }
            }

            return "";
        }

        public new Article makeNewArticleByCloningContent()
        {
            Article a = new Article();
            a.BaseArticleID = BaseArticleID;
            a.categoryID = categoryID;
            a.Url = Url;
            a.Excerpt = Excerpt;
            a.Desc = Desc;
            a.Name = Name;
            a.Slug = Slug;
            a.Keywords = Keywords;
            a.MetaData = MetaData;
            a.MetaKeywords = MetaKeywords;
            a.Lang = Lang;
            return a;
        }
    }
}