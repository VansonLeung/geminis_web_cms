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
    public class ContentPageDbContext
    {
        // singleton

        private static ContentPageDbContext contentPageDbContext;

        public static ContentPageDbContext getInstance()
        {
            if (contentPageDbContext == null)
            {
                contentPageDbContext = new ContentPageDbContext();
            }
            return contentPageDbContext;
        }


        // initialization

        private BaseDbContext db = BaseDbContext.getInstance();

        protected DbSet<ContentPage> getArticleDb()
        {
            return db.contentPageDb;
        }






        // methods

        public List<ContentPage> findArticles()
        {
            return (getArticleDb()).Include(acc => acc.createdByAccount)
                .Include(acc => acc.approvedByAccount)
                .Include(acc => acc.publishedByAccount).ToList();
        }

        public List<ContentPage> findArticlesGroupByBaseVersion(string lang = "en")
        {
            if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
            {
                return getArticleDb()
                    .GroupBy(acc => acc.BaseArticleID)
                    .Select(u => u.Where(acc => acc.Lang == lang
                    ).OrderByDescending(acc => acc.Version)
                    .FirstOrDefault())
                    .OrderByDescending(acc => acc.modified_at)
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

                return getArticleDb()
                    .GroupBy(acc => acc.BaseArticleID)
                    .Select(u => u.Where(acc => acc.Lang == lang
                    ).OrderByDescending(acc => acc.Version)
                    .FirstOrDefault())
                    .Where(acc => categories.Contains(acc.categoryID ?? 0))
                    .OrderByDescending(acc => acc.modified_at)
                    .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
                    .Include(acc => acc.category)
                    .ToList();
            }

            return new List<ContentPage>();
        }


        public ContentPage findArticleByID(int articleID)
        {
            return getArticleDb().Where(acc => acc.ArticleID == articleID).FirstOrDefault();
        }

        public List<ContentPage> findArticlesByCategoryID(int categoryID)
        {
            return (getArticleDb().AsNoTracking().Where(acc => acc.categoryID == categoryID)).Include(acc => acc.createdByAccount)
                .Include(acc => acc.approvedByAccount)
                .Include(acc => acc.publishedByAccount).ToList();
        }


        public List<ContentPage> findArticlesRequestingApproval()
        {
            return getArticleDb().Where(acc =>
            acc.isRequestingApproval == true)
            .OrderByDescending(acc => acc.modified_at)
            .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
            .Include(acc => acc.category)
            .ToList();
        }

        public List<ContentPage> findArticlesGroupByBaseVersionRequestingApproval(string lang = "en")
        {
            return getArticleDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .Where(acc => acc.isRequestingApproval == true)
                .OrderByDescending(acc => acc.modified_at)
                .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category)
                .ToList();
        }

        public List<ContentPage> findArticlesGroupByBaseVersionApproved(string lang = "en")
        {
            return getArticleDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .Where(acc => acc.isApproved == true)
                .OrderByDescending(acc => acc.dateApproved)
                .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category)
                .ToList();
        }


        public ContentPage findArticleByVersionAndLang(int baseArticleID, int version = 0, String lang = null)
        {
            if (lang == null)
            {
                return null;
            }

            if (version == 0)
            {
                return getArticleDb().Where(acc =>
                acc.BaseArticleID == baseArticleID &&
                acc.Lang == lang)
                .OrderByDescending(acc => acc.Version)
                .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category)
                .FirstOrDefault();
            }
            else
            {
                return getArticleDb().Where(acc =>
                acc.BaseArticleID == baseArticleID &&
                acc.Version == version &&
                acc.Lang == lang).FirstOrDefault();
            }
        }

        public ContentPage findArticleByArticle(ContentPage article)
        {
            return null;
            //   return articleDb.Where(acc => acc.Username == account.Username && acc.Password == account.Password).FirstOrDefault();
        }


        public ContentPage findLatestArticleByBaseArticle(ContentPage article, String lang = null)
        {
            var targetBaseArticleID = article.BaseArticleID;
            var targetLang = article.Lang;

            var baseLatestArticle = getArticleDb().Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
            .Include(acc => acc.category)
            .FirstOrDefault();


            if (baseLatestArticle != null)
            {
                return baseLatestArticle;
            }

            return null;
        }


        public ContentPage findLatestArticleByBaseArticleID(int baseArticleID, String lang = null)
        {
            var targetBaseArticleID = baseArticleID;
            var targetLang = lang;

            var baseLatestArticle = getArticleDb().Where(acc =>
            acc.BaseArticleID == baseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
            .Include(acc => acc.category)
            .FirstOrDefault();


            if (baseLatestArticle != null)
            {
                return baseLatestArticle;
            }

            return null;
        }



        public List<ContentPage> findAllArticlesByBaseArticle(ContentPage article, String lang = null)
        {
            var articles = getArticleDb().Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
            .Include(acc => acc.category)
            .ToList();

            return articles;
        }





        public List<ContentPage> findAllArticlesByBaseArticleIncludingAllLocales(ContentPage article)
        {
            if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
            {
                var articles = getArticleDb().Where(acc =>
                acc.BaseArticleID == article.BaseArticleID
                )
                .OrderByDescending(acc => acc.Version)
                .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category)
                .ToList();

                return articles;
            }
            if (SessionPersister.account != null)
            {
                var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();
                categories.Add(0);

                var articles = getArticleDb().Where(acc =>
                acc.BaseArticleID == article.BaseArticleID
                    && categories.Contains(acc.categoryID ?? 0)
                )
                .OrderByDescending(acc => acc.Version)
                .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category)
                .ToList();

                return articles;
            }

            return new List<ContentPage>();
        }




        public List<ContentPage> findAllLocaleArticlesByBaseArticleAndVersion(ContentPage article, String Lang = "en")
        {
            var articles = getArticleDb().Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang != Lang &&
            acc.Version == article.Version
            )
            .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
            .Include(acc => acc.category)
            .ToList();

            return articles;
        }




        public bool articleWithSameVersionAndLangAlreadyPresents(ContentPage article)
        {
            return findArticleByVersionAndLang(article.BaseArticleID, article.Version, article.Lang) != null;
        }







        // ARTICLE EDITOR ONLY



        public String tryCreateNewArticle(ContentPage article)
        {
            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(article);
            if (error != null)
            {
                return error;
            }

            ContentPage latestArticle = null;

            if (article.categoryID == -1)
            {
                article.categoryID = null;
            }

            if (article.BaseArticleID != 0)
            {
                if (String.IsNullOrEmpty(article.Lang))
                {
                    article.Lang = "en";
                }

                if (!article.Lang.Equals("en"))
                {
                    return tryCreateNewLocaleArticle(article);
                }

                latestArticle = findLatestArticleByBaseArticle(article, null);
                article = latestArticle.makeNewContentPageByCloningContent();
                article.Version = latestArticle.Version;
                article.Version = article.Version + 1;
            }
            else
            {
                article.Version = 1;
            }

            if (String.IsNullOrEmpty(article.Lang))
            {
                article.Lang = "en";
            }

            if (articleWithSameVersionAndLangAlreadyPresents(article))
            {
                return "Article already presents";
            }

            article.createdBy = SessionPersister.account.AccountID;
            getArticleDb().Add(article);
            db.SaveChanges();


            if (article.BaseArticleID == 0)
            {
                db.Entry(article).State = EntityState.Modified;
                article.BaseArticleID = article.ArticleID;
                db.SaveChanges();
            }


            // try clone new locale for this new article
            if (latestArticle != null && article != null)
            {
                tryCloningNewLocaleArticleForNewArticleVersion(latestArticle, article);
            }

            if (article.Version == 1)
            {
                AuditLogDbContext.getInstance().createAuditLogContentPageAction(article, AuditLogDbContext.ACTION_CREATE);
            }
            else
            {
                AuditLogDbContext.getInstance().createAuditLogContentPageAction(article, AuditLogDbContext.ACTION_CREATE_NEW_VERSION);
            }

            return null;
        }


        void tryCloningNewLocaleArticleForNewArticleVersion(ContentPage latestArticle, ContentPage newArticle)
        {
            // try clone new locale for this new article
            var articles = findAllLocaleArticlesByBaseArticleAndVersion(latestArticle, latestArticle.Lang);
            foreach (var _a in articles)
            {
                ContentPage _new = _a.makeNewContentPageByCloningContent();
                _new.Version = newArticle.Version;
                _new.createdBy = SessionPersister.account.AccountID;
                getArticleDb().Add(_new);
            }
            db.SaveChanges();
        }


        public String tryCreateNewLocaleArticle(ContentPage article)
        {
            if (article.BaseArticleID != 0)
            {
                if (article.Version == 0)
                {
                    var latestArticle = findLatestArticleByBaseArticle(article, article.Lang);
                    article.Version = latestArticle.Version;

                    var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(latestArticle);
                    if (error != null)
                    {
                        return error;
                    }
                }
            }

            if (articleWithSameVersionAndLangAlreadyPresents(article))
            {
                return "ContentPage with the same version and language already presents";
            }

            article.createdBy = SessionPersister.account.AccountID;
            getArticleDb().Add(article);
            db.SaveChanges();


            if (article.BaseArticleID == 0)
            {
                db.Entry(article).State = EntityState.Modified;
                article.BaseArticleID = article.ArticleID;
                db.SaveChanges();
            }

            return null;
        }

        public String tryEditArticle(ContentPage article)
        {
            if (article.categoryID == -1)
            {
                article.categoryID = null;
            }

            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                return ResHelper.S("itemisfrozen");
            }

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.Name = article.Name;
            _article.Desc = article.Desc;
            _article.Url = article.Url;
            _article.Keywords = article.Keywords;
            _article.MetaData = article.MetaData;
            _article.MetaKeywords = article.MetaKeywords;
            _article.Excerpt = article.Excerpt;
            db.SaveChanges();

            return null;
        }



        public String tryEditArticleProperties(ContentPage article, bool allLocales)
        {
            if (article.categoryID == -1)
            {
                article.categoryID = null;
            }

            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                return ResHelper.S("itemisfrozen");
            }

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.Url = article.Url;
            _article.Slug = article.Slug;
            _article.categoryID = article.categoryID;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.Url = article.Url;
                    _article.Slug = article.Slug;
                    _article.categoryID = article.categoryID;
                }
            }

            db.SaveChanges();
            return null;
        }


        public String tryDeleteArticle(ContentPage article)
        {
            var _article = findArticleByID(article.ArticleID);

            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                return ResHelper.S("itemisfrozen");
            }

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            getArticleDb().Remove(article);
            db.SaveChanges();

            return null;
        }






















        // ARTICLE EDITOR REQUEST FOR APPROVAL

        public String trySubmitRequestForApproval(ContentPage article, bool allLocales)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                return ResHelper.S("itemisfrozen");
            }

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.isRequestingApproval = true;
            _article.isFrozen = true;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _article.isRequestingApproval = true;
                    _article.isFrozen = true;
                }
            }

            db.SaveChanges();

            return null;
        }




        // ARTICLE APPROVER ONLY

        public String tryRequestApproval(ContentPage article, bool allLocales)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }

            if (_article.isApproved)
            {
                return "Item is already approved";
            }

            if (_article.isUnapproved)
            {
                return "item is already unapproved";
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.isApproved = true;
            _article.isUnapproved = false;
            _article.isFrozen = true;
            _article.dateApproved = DateTime.UtcNow;
            _article.approvalRemarks = article.approvalRemarks;
            _article.approvedBy = SessionPersister.account.AccountID;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isApproved = true;
                    _a.isUnapproved = false;
                    _a.isFrozen = true;
                    _a.dateApproved = DateTime.UtcNow;
                    _a.approvalRemarks = article.approvalRemarks;
                    _a.approvedBy = SessionPersister.account.AccountID;
                }
            }

            db.SaveChanges();

            AuditLogDbContext.getInstance().createAuditLogContentPageAction(article, AuditLogDbContext.ACTION_APPROVE);

            return null;
        }

        public String tryRequestUnapproval(ContentPage article, bool allLocales)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }

            if (_article.isApproved)
            {
                return "Item is already approved";
            }

            if (_article.isUnapproved)
            {
                return "item is already unapproved";
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.isApproved = false;
            _article.isUnapproved = true;
            _article.isFrozen = true;
            _article.dateApproved = null;
            _article.dateUnapproved = DateTime.UtcNow;
            _article.approvalRemarks = article.approvalRemarks;
            _article.approvedBy = SessionPersister.account.AccountID;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isApproved = false;
                    _a.isUnapproved = true;
                    _a.isFrozen = true;
                    _a.dateApproved = null;
                    _a.dateUnapproved = DateTime.UtcNow;
                    _a.approvalRemarks = article.approvalRemarks;
                    _a.approvedBy = SessionPersister.account.AccountID;
                }
            }

            db.SaveChanges();

            AuditLogDbContext.getInstance().createAuditLogContentPageAction(article, AuditLogDbContext.ACTION_UNAPPROVE);

            return null;
        }


    }
}