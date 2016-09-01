using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Helpers;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class ArticlePublishedDbContext
    {
        #region "Constructor"

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

        private BaseDbContext db = BaseDbContext.getInstance();

        protected virtual DbSet<ArticlePublished> getArticlePublishedDb()
        {
            return db.articlePublishedDb;
        }

        #endregion



        // methods

        #region "Query"

        public List<ArticlePublished> findPublishedArticlesGroupByBaseVersion(string lang = "en")
        {
            if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
            {
                return getArticlePublishedDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang
                ).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .OrderByDescending(acc => acc.datePublished)
                .Include(acc => acc.createdByAccount)
                .Include(acc => acc.category)
                .ToList();
            }

            if (SessionPersister.account != null)
            {
                var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();
                categories.Add(0);

                return getArticlePublishedDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang
                ).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .Where(acc => categories.Contains(acc.categoryID ?? 0))
                .OrderByDescending(acc => acc.datePublished)
                .Include(acc => acc.createdByAccount)
                .Include(acc => acc.category)
                .ToList();
            }

            return new List<ArticlePublished>();
        }

        #endregion

        #region "Internal"

        protected string deletePublishedArticlesByBaseArticle(Article article)
        {
            var targetBaseArticleID = article.BaseArticleID;

            getArticlePublishedDb().RemoveRange(
                getArticlePublishedDb().Where(acc =>
                acc.BaseArticleID == targetBaseArticleID
                )
            );
            db.SaveChanges();

            return null;
        }


        protected string addArticleToPublished(Article article)
        {
            var _article = ArticlePublished.makeNewArticleByCloningContentAndVersion(article);
            _article.isPublished = true;
            _article.datePublished = DateTime.UtcNow;
            _article.datePublishStart = article.datePublishStart;
            _article.datePublishEnd = article.datePublishEnd;
            _article.publishedBy = SessionPersister.account.AccountID;
            getArticlePublishedDb().Add(_article);

            var articles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article);
            foreach (var __article in articles)
            {
                var ___article = ArticlePublished.makeNewArticleByCloningContentAndVersion(__article);
                ___article.isPublished = true;
                ___article.datePublished = DateTime.UtcNow;
                ___article.datePublishStart = article.datePublishStart;
                ___article.datePublishEnd = article.datePublishEnd;
                ___article.publishedBy = SessionPersister.account.AccountID;
                getArticlePublishedDb().Add(___article);
            }
            db.SaveChanges();

            return null;
        }

        #endregion


        // ARTICLE PUBLISHER ONLY

        #region "Publish / Unpublish"

        public string tryPublishArticle(Article article, bool allLocales)
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

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            deletePublishedArticlesByBaseArticle(article);
            addArticleToPublished(article);

            var local = ArticleDbContext.getInstance().getArticleDb()
                            .Local
                            .FirstOrDefault(f => f.BaseArticleID == _article.BaseArticleID);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }



            var allArticles = ArticleDbContext.getInstance().findArticlesGroupByBaseVersionApproved();
            foreach (Article a in allArticles)
            {
                db.Entry(a).State = EntityState.Modified;
                a.isPublished = false;
                a.datePublished = null;
                a.publishedBy = null;

                if (allLocales)
                {
                    var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(a, a.Lang);
                    foreach (var _a in _localeArticles)
                    {
                        db.Entry(_a).State = EntityState.Modified;
                        _a.isPublished = false;
                        _a.datePublished = null;
                        _a.publishedBy = null;
                    }
                }
            }

            db.SaveChanges();


            db.Entry(_article).State = EntityState.Modified;
            _article.isPublished = true;
            _article.datePublished = DateTime.UtcNow;
            _article.datePublishStart = article.datePublishStart;
            _article.datePublishEnd = article.datePublishEnd;
            _article.publishedBy = SessionPersister.account.AccountID;

            if (allLocales)
            {
                var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isPublished = true;
                    _a.datePublished = DateTime.UtcNow;
                    _a.datePublishStart = article.datePublishStart;
                    _a.datePublishEnd = article.datePublishEnd;
                    _a.publishedBy = SessionPersister.account.AccountID;
                }
            }

            db.SaveChanges();

            return null;
        }

        public string tryUnpublishArticle(Article article, bool allLocales)
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

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            deletePublishedArticlesByBaseArticle(article);
            
            db.Entry(_article).State = EntityState.Modified;
            _article.isPublished = false;
            _article.datePublished = null;
            _article.publishedBy = null;

            if (allLocales)
            {
                var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isPublished = false;
                    _a.datePublished = null;
                    _a.publishedBy = null;
                }
            }

            db.SaveChanges();

            return null;
        }

        #endregion
    }
}