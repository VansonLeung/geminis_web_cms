using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class ContentPagePublishedDbContext
    {
        // singleton

        private static ContentPagePublishedDbContext contentPagePublishedDbContext;

        public static ContentPagePublishedDbContext getInstance()
        {
            if (contentPagePublishedDbContext == null)
            {
                contentPagePublishedDbContext = new ContentPagePublishedDbContext();
            }
            return contentPagePublishedDbContext;
        }


        // initializations 

        private BaseDbContext db = new BaseDbContext();

        protected virtual DbSet<ContentPagePublished> getArticlePublishedDb()
        {
            return db.contentPagePublishedDb;
        }



        // methods

        public List<ContentPagePublished> findPublishedArticlesGroupByBaseVersion(string lang = "en")
        {
            return getArticlePublishedDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .OrderByDescending(acc => acc.datePublished)
                .Include(acc => acc.createdByAccount)
                .ToList();
        }

        public void deletePublishedArticlesByBaseArticle(ContentPage article)
        {
            var targetBaseArticleID = article.BaseArticleID;

            getArticlePublishedDb().RemoveRange(
                getArticlePublishedDb().Where(acc =>
                acc.BaseArticleID == targetBaseArticleID
                )
            );
            db.SaveChanges();
        }


        public void addArticleToPublished(ContentPage article)
        {
            var _article = ContentPagePublished.makeNewContentPagePublishedByCloningContent(article);
            _article.isPublished = true;
            _article.datePublished = DateTime.UtcNow;
            getArticlePublishedDb().Add(_article);

            var articles = ContentPageDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article);
            foreach (var __article in articles)
            {
                var ___article = ContentPagePublished.makeNewContentPagePublishedByCloningContent(__article);
                ___article.isPublished = true;
                ___article.datePublished = DateTime.UtcNow;
                getArticlePublishedDb().Add(___article);
            }
            db.SaveChanges();
        }



        // ARTICLE PUBLISHER ONLY

        public String tryPublishArticle(ContentPage article, bool allLocales)
        {
            var _article = ContentPageDbContext.getInstance().findArticleByVersionAndLang(article.BaseArticleID, article.Version, "en");
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
                var _localeArticles = ContentPageDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
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

        public String tryUnpublishArticle(ContentPage article, bool allLocales)
        {
            var _article = ContentPageDbContext.getInstance().findArticleByVersionAndLang(article.BaseArticleID, article.Version, article.Lang);
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
                var _localeArticles = ContentPageDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
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