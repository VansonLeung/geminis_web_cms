using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Context
{
    public class ArticlePublishedDbContext : DbContext
    {
        public DbSet<Article> articleDb { get; set; }


        public List<Article> findPublishedArticlesGroupByBaseVersion()
        {
            return articleDb
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == "en").OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .OrderByDescending(acc => acc.modified_at)
                .ToList();
        }

        public void deletePublishedArticlesByBaseArticle(Article article)
        {
            var targetBaseArticleID = article.BaseArticleID;
         
            articleDb.RemoveRange(
                articleDb.Where(acc =>
                acc.BaseArticleID == targetBaseArticleID
                )
            );
            SaveChanges();
        }


        public void addArticleToPublished(Article article, ArticleDbContext articleDbContext)
        {
            article = articleDb.Add(article);

            Entry(article).State = EntityState.Modified;
            article.isPublished = true;
            article.datePublished = DateTime.UtcNow;
            SaveChanges();

            var articles = articleDbContext.findAllLocaleArticlesByBaseArticleAndVersion(article);
            foreach (var _article in articles)
            {
                var __article = articleDb.Add(_article);

                Entry(__article).State = EntityState.Modified;
                __article.isPublished = true;
                __article.datePublished = DateTime.UtcNow;
                SaveChanges();

            }
            SaveChanges();
        }



        // ARTICLE PUBLISHER ONLY

        public String tryPublishArticle(Article article, ArticleDbContext articleDbContext)
        {
            deletePublishedArticlesByBaseArticle(article);
            addArticleToPublished(article, articleDbContext);

            var _article = articleDbContext.findArticleByVersionAndLang(article.BaseArticleID, article.Version, "en");
            if (_article == null)
            {
                return "Item not found";
            }

            Entry(_article).State = EntityState.Modified;
            _article.isPublished = true;
            _article.datePublished = DateTime.UtcNow;
            SaveChanges();
            return null;
        }

        public String tryUnpublishArticle(Article article, ArticleDbContext articleDbContext)
        {
            deletePublishedArticlesByBaseArticle(article);

            var _article = articleDbContext.findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }

            Entry(_article).State = EntityState.Modified;
            _article.isPublished = false;
            _article.datePublished = null;
            SaveChanges();
            return null;
        }
    }
}