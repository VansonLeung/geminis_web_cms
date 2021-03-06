﻿using System;
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
            a.isUnapproved = article.isUnapproved;
            a.dateApproved = article.dateApproved;
            a.dateUnapproved = article.dateUnapproved;
            a.isPublished = article.isPublished;
            a.datePublished = article.datePublished;
            a.Version = article.Version;
            a.Url = article.Url;
            a.Excerpt = article.Excerpt;
            a.BaseArticleID = article.BaseArticleID;
            a.categoryID = article.categoryID;
            a.Desc = article.Desc;
            a.Name = article.Name;
            a.Slug = article.Slug;
            a.Keywords = article.Keywords;
            a.MetaData = article.MetaData;
            a.MetaKeywords = article.MetaKeywords;
            a.Lang = article.Lang;
            a.datePublishStart = article.datePublishStart;
            a.datePublishEnd = article.datePublishEnd;
            return a;
        }
    }
}