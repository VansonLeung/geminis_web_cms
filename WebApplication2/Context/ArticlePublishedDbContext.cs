using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Helpers;
using WebApplication2.Models;
using WebApplication2.Models.Infrastructure;
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


        #endregion



        // methods

        #region "Query"

        public List<ArticlePublished> findPublishedArticlesGroupByBaseVersion(string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
                {
                    return db.articlePublishedDb
                    .GroupBy(acc => acc.BaseArticleID)
                    .Select(u => u.Where(acc => acc.Lang == lang
                    ).OrderByDescending(acc => acc.Version)
                    .FirstOrDefault())
                    .OrderByDescending(acc => acc.datePublished)
                    .Include(acc => acc.createdByAccount)
    .Include(acc => acc.approvedByAccount)
    .Include(acc => acc.publishedByAccount)
                    .Include(acc => acc.category)
                    .ToList();
                }

                if (SessionPersister.account != null)
                {
                    var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();
                    categories.Add(0);

                    return db.articlePublishedDb
                    .GroupBy(acc => acc.BaseArticleID)
                    .Select(u => u.Where(acc => acc.Lang == lang
                    ).OrderByDescending(acc => acc.Version)
                    .FirstOrDefault())
                    .Where(acc => categories.Contains(acc.categoryID ?? 0))
                    .OrderByDescending(acc => acc.datePublished)
                    .Include(acc => acc.createdByAccount)
    .Include(acc => acc.approvedByAccount)
    .Include(acc => acc.publishedByAccount)
                    .Include(acc => acc.category)
                    .ToList();
                }

                return new List<ArticlePublished>();
            }
        }

        #endregion

        #region "Internal"

        protected string deletePublishedArticlesByBaseArticle(Article article)
        {
            var targetBaseArticleID = article.BaseArticleID;

            using (var db = new BaseDbContext())
            {
                db.articlePublishedDb.RemoveRange(
                db.articlePublishedDb.Where(acc =>
                acc.BaseArticleID == targetBaseArticleID
                )
            );
                db.SaveChanges();

                return null;
            }
        }


        protected string addArticleToPublished(Article article)
        {
            using (var db = new BaseDbContext())
            {
                var _article = ArticlePublished.makeNewArticleByCloningContentAndVersion(article);
                _article.isPublished = true;
                _article.datePublished = DateTime.UtcNow;
                _article.datePublishStart = article.datePublishStart;
                _article.datePublishEnd = article.datePublishEnd;
                _article.publishedBy = SessionPersister.account.AccountID;
                db.articlePublishedDb.Add(_article);

                var articles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article);
                foreach (var __article in articles)
                {
                    var ___article = ArticlePublished.makeNewArticleByCloningContentAndVersion(__article);
                    ___article.isPublished = true;
                    ___article.datePublished = DateTime.UtcNow;
                    ___article.datePublishStart = article.datePublishStart;
                    ___article.datePublishEnd = article.datePublishEnd;
                    ___article.publishedBy = SessionPersister.account.AccountID;
                    db.articlePublishedDb.Add(___article);
                }
                db.SaveChanges();

                return null;
            }
        }




        public List<ArticlePublished> getArticlesPublishedByCategory(Category category, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                return (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.categoryID == category.ItemID
                && acc.Lang == lang
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderByDescending(acc => acc.ArticleID))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).ToList();
            }
        }




        public bool hasArticlesPublishedByCategory(Category category, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                return (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.categoryID == category.ItemID
                && acc.Lang == lang
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderByDescending(acc => acc.ArticleID))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).Count() > 0;
            }
        }




        public int getArticlesPublishedByCategoryTotalCount(Category category, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                var query = (db.articlePublishedDb.AsNoTracking().Where(acc =>
                    acc.categoryID == category.ItemID
                    && acc.Lang == lang
                    && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                    && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ));

                int totalCount = query.Count();

                return totalCount;
            }
        }
        public List<ArticlePublished> getArticlesPublishedByCategoryPaginated(Category category, int size = 10, int page = 1, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                var query = (db.articlePublishedDb.AsNoTracking().Where(acc =>
                    acc.categoryID == category.ItemID
                    && acc.Lang == lang
                    && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                    && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ));

                var items = query
                    .OrderByDescending(acc => acc.datePublished)
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount)
                    .Skip((int)size * (page - 1))
                    .Take((int)size)
                    .ToList();

                return items;
            }
        }


        public ArticlePublished getArticlePublishedByBaseArticleID(int baseArticleID, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                return (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.BaseArticleID == baseArticleID
                && acc.Lang == lang
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderByDescending(acc => acc.Version))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).FirstOrDefault();
            }
        }


        public ArticlePublished getArticlePublishedBySlugAndCategoryID(int categoryID, string slug, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                return (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.categoryID == categoryID
                && acc.Slug == slug
                && acc.Lang == lang
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderByDescending(acc => acc.Version))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).FirstOrDefault();
            }
        }


        public ArticlePublished getPrevArticlePublishedBySlugAndCategoryID(int categoryID, string slug, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                var articlePublished = (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.categoryID == categoryID
                && acc.Slug == slug
                && acc.Lang == lang
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderByDescending(acc => acc.Version))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).FirstOrDefault();

                var prevArticlePublished = (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.categoryID == categoryID
                && acc.Lang == lang
                && acc.BaseArticleID != articlePublished.BaseArticleID
                && acc.datePublished <= articlePublished.datePublished
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderByDescending(acc => acc.datePublished))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).FirstOrDefault();

                return prevArticlePublished;
            }
        }


        public ArticlePublished getNextArticlePublishedBySlugAndCategoryID(int categoryID, string slug, string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                var now = DateTime.Now;

                var articlePublished = (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.categoryID == categoryID
                && acc.Slug == slug
                && acc.Lang == lang
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderByDescending(acc => acc.Version))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).FirstOrDefault();

                var nextArticlePublished = (db.articlePublishedDb.AsNoTracking().Where(acc =>
                acc.categoryID == categoryID
                && acc.Lang == lang
                && acc.BaseArticleID != articlePublished.BaseArticleID
                && acc.datePublished >= articlePublished.datePublished
                && (!acc.datePublishStart.HasValue || acc.datePublishStart.Value <= now)
                && (!acc.datePublishEnd.HasValue || acc.datePublishEnd.Value >= now)
                ).OrderBy(acc => acc.datePublished))
                    .Include(acc => acc.createdByAccount)
                    .Include(acc => acc.approvedByAccount)
                    .Include(acc => acc.publishedByAccount).FirstOrDefault();

                return nextArticlePublished;
            }
        }


        #endregion


        // ARTICLE PUBLISHER ONLY

        #region "Publish / Unpublish"

        public string tryPublishArticle(Article article, bool allLocales)
        {
            using (var db = new BaseDbContext())
            {
                var _article = article;
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

                var local = db.articleDb
                                .Local
                                .FirstOrDefault(f => f.BaseArticleID == _article.BaseArticleID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
            }

            using (var db = new BaseDbContext())
            {
                var _article = article;
                db.Entry(_article).State = EntityState.Modified;
                _article.isPublished = true;
                _article.datePublished = DateTime.UtcNow;
                _article.datePublishStart = article.datePublishStart;
                _article.datePublishEnd = article.datePublishEnd;
                _article.publishedBy = SessionPersister.account.AccountID;

                if (allLocales)
                {
                    var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang, db);
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


                var allArticles = ArticleDbContext.getInstance().findArticlesGroupByBaseVersionApproved(_article.BaseArticleID, "en", db);
                foreach (Article a in allArticles)
                {
                    if (_article.ArticleID == a.ArticleID)
                    {
                        continue;
                    }

                    db.Entry(a).State = EntityState.Modified;
                    a.isPublished = false;
                    a.datePublished = null;
                    a.publishedBy = null;

                    if (allLocales)
                    {
                        var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(a, a.Lang, db);
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

                AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_PUBLISH);

                return null;
            }           
            
        }

        public string tryUnpublishArticle(Article article, bool allLocales)
        {
            using (var db = new BaseDbContext())
            {

                var _article = article;
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

                var local = db.articleDb
                                .Local
                                .FirstOrDefault(f => f.BaseArticleID == _article.BaseArticleID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(_article).State = EntityState.Modified;
                _article.isPublished = false;
                _article.datePublished = null;
                _article.publishedBy = null;
                db.SaveChanges();

                if (allLocales)
                {
                    var _localeArticles = ArticleDbContext.getInstance().findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang, db);
                    foreach (var _a in _localeArticles)
                    {
                        db.Entry(_a).State = EntityState.Modified;
                        _a.isPublished = false;
                        _a.datePublished = null;
                        _a.publishedBy = null;
                        db.SaveChanges();
                    }
                }

                AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_UNPUBLISH);

                return null;
            }
        }

        #endregion
    }
}