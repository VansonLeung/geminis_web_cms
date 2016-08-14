using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class ContentPagePublished : BaseArticle
    {
        public static ContentPagePublished makeNewContentPagePublishedByCloningContent(BaseArticle article)
        {
            ContentPagePublished a = new ContentPagePublished();
            a.isApproved = article.isApproved;
            a.dateApproved = article.dateApproved;
            a.isPublished = article.isPublished;
            a.datePublished = article.datePublished;
            a.Version = article.Version;
            a.Url = article.Url;
            a.Excerpt = article.Excerpt;
            a.BaseArticleID = article.BaseArticleID;
            a.Desc = article.Desc;
            a.Name = article.Name;
            a.Slug = article.Slug;
            a.Keywords = article.Keywords;
            a.Lang = article.Lang;
            a.datePublishStart = article.datePublishStart;
            a.datePublishEnd = article.datePublishEnd;
            a.createdBy = article.createdBy;
            a.approvedBy = article.approvedBy;
            a.publishedBy = article.publishedBy;
            return a;
        }
    }
}