using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class ArticlePublishedDbContext
    {
        // singleton

        private static ArticlePublishedDbContext articlePublishedDbContext;

        public static ArticlePublishedDbContext getInstance()
        {
            if (articlePublishedDbContext == null)
            {
                articlePublishedDbContext = new ArticlePublishedDbContext();
            }
            return articlePublishedDbContext;
        }


        // initializations 

        private BaseDbContext db = new BaseDbContext();

        protected virtual DbSet<ArticlePublished> getArticlePublishedDb()
        {
            return db.articlePublishedDb;
        }



        // methods

        public List<ArticlePublished> findPublishedArticlesGroupByBaseVersion(string lang = "en")
        {
            return getArticlePublishedDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .OrderByDescending(acc => acc.datePublished)
                .Include(acc => acc.createdByAccount)
                .ToList();
        }

        public void deletePublishedArticlesByBaseArticle(Article article)
        {
            var targetBaseArticleID = article.BaseArticleID;

            getArticlePublishedDb().RemoveRange(
                getArticlePublishedDb().Where(acc =>
                acc.BaseArticleID == targetBaseArticleID
                )
            );
            db.SaveChanges();
        }


        public void addArticleToPublished(Article article)
        {
            var _article = ArticlePublished.makeNewArticleByCloningContentAndVersion(article);
            _article.isPublished = true;
            _article.datePublished = DateTime.UtcNow;
            getArticlePublishedDb().Add(_article);

            var articles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article);
            foreach (var __article in articles)
            {
                var ___article = ArticlePublished.makeNewArticleByCloningContentAndVersion(__article);
                ___article.isPublished = true;
                ___article.datePublished = DateTime.UtcNow;
                getArticlePublishedDb().Add(___article);
            }
            db.SaveChanges();
        }



        // ARTICLE PUBLISHER ONLY

        public String tryPublishArticle(Article article, bool allLocales)
        {
            var _article = ArticleDbContext.getInstance().findArticleByVersionAndLang(article.BaseArticleID, article.Version, "en");
            if (_article == null)
            {
                return "Item not found";
            }
            if (!_article.isApproved)
            {
                return "Item not approved";
            }

            deletePublishedArticlesByBaseArticle(article);
            addArticleToPublished(article);

            db.Entry(_article).State = EntityState.Modified;
            _article.isPublished = true;
            _article.datePublished = DateTime.UtcNow;
            _article.publishedBy = SessionPersister.account.AccountID;

            if (allLocales)
            {
                var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isPublished = true;
                    _a.datePublished = DateTime.UtcNow;
                    _a.publishedBy = SessionPersister.account.AccountID;
                }
            }

            db.SaveChanges();

            return null;
        }

        public String tryUnpublishArticle(Article article, bool allLocales)
        {
            var _article = ArticleDbContext.getInstance().findArticleByVersionAndLang(article.BaseArticleID, article.Version, article.Lang);
            if (_article == null)
            {
                return "Item not found";
            }
            if (!_article.isApproved)
            {
                return "Item not approved";
            }

            deletePublishedArticlesByBaseArticle(article);
            
            db.Entry(_article).State = EntityState.Modified;
            _article.isPublished = false;
            _article.datePublished = null;
            _article.publishedBy = SessionPersister.account.AccountID;

            if (allLocales)
            {
                var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isPublished = false;
                    _a.datePublished = null;
                    _a.publishedBy = SessionPersister.account.AccountID;
                }
            }

            db.SaveChanges();

            return null;
        }
    }
}