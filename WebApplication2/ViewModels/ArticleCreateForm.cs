using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.Infrastructure;

namespace WebApplication2.ViewModels
{
    public class ArticleCreateForm
    {
        public int? categoryID { get; set; }
        [ForeignKey("categoryID")]
        public virtual Category category { get; set; }


        public string Name_en { get; set; }
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Desc_en { get; set; }
        [DataType(DataType.MultilineText)]
        public string Excerpt_en { get; set; }
        public string Keywords_en { get; set; }
        public string MetaData_en { get; set; }
        public string MetaKeywords_en { get; set; }


        public string Name_zh { get; set; }
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Desc_zh { get; set; }
        [DataType(DataType.MultilineText)]
        public string Excerpt_zh { get; set; }
        public string Keywords_zh { get; set; }
        public string MetaData_zh { get; set; }
        public string MetaKeywords_zh { get; set; }


        public string Name_cn { get; set; }
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Desc_cn { get; set; }
        [DataType(DataType.MultilineText)]
        public string Excerpt_cn { get; set; }
        public string Keywords_cn { get; set; }
        public string MetaData_cn { get; set; }
        public string MetaKeywords_cn { get; set; }



        public Article makeBaseArticle()
        {
            Article article = new Article();
            article.Name = Name_en;
            article.Desc = Desc_en;
            article.Excerpt = Excerpt_en;
            article.Keywords = Keywords_en;
            article.MetaData = MetaData_en;
            article.MetaKeywords = MetaKeywords_en;
            article.categoryID = categoryID;
            if (article.categoryID == -1)
            {
                article.categoryID = null;
            }
            return article;
        }

        public Article makeLocaleArticle(string lang)
        {
            if (lang.Equals("zh"))
            {
                Article article = new Article();
                article.Name = Name_zh;
                article.Desc = Desc_zh;
                article.Excerpt = Excerpt_zh;
                article.Keywords = Keywords_zh;
                article.MetaData = MetaData_zh;
                article.MetaKeywords = MetaKeywords_zh;
                article.categoryID = categoryID;
                if (article.categoryID == -1)
                {
                    article.categoryID = null;
                }
                article.Lang = "zh";
                return article;
            }
            if (lang.Equals("cn"))
            {
                Article article = new Article();
                article.Name = Name_cn;
                article.Desc = Desc_cn;
                article.Excerpt = Excerpt_cn;
                article.Keywords = Keywords_cn;
                article.MetaData = MetaData_cn;
                article.MetaKeywords = MetaKeywords_cn;
                article.categoryID = categoryID;
                if (article.categoryID == -1)
                {
                    article.categoryID = null;
                }
                article.Lang = "cn";
                return article;
            }
            return null;
        }
        
    }
}