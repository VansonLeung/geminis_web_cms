using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class ContentPage : BaseArticle
    {
        public ContentPage makeNewContentPageByCloningContent()
        {
            ContentPage a = new ContentPage();
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
            a.createdBy = createdBy;
            a.approvedBy = approvedBy;
            a.publishedBy = publishedBy;
            return a;
        }
    }
}