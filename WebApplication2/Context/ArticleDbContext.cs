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
    public class ArticleDbContext
    {
        #region "Constructor"

        // singleton

        private static ArticleDbContext articleDbContext;

        public static ArticleDbContext getInstance()
        {
            if (articleDbContext == null)
            {
                articleDbContext = new ArticleDbContext();
            }
            return articleDbContext;
        }


        // initialization


        #endregion



        // methods

        #region "Query"

        public List<Article> findArticles()
        {
            using (var db = new BaseDbContext())
            {
                return (db.articleDb).Include(acc => acc.createdByAccount)
                .Include(acc => acc.approvedByAccount)
                .Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category).ToList();
            }
        }

        public List<Article> findArticlesByCategoryID(int categoryID)
        {
            using (var db = new BaseDbContext())
            {
                return (db.articleDb.AsNoTracking().Where(acc => acc.categoryID == categoryID)).Include(acc => acc.createdByAccount)
                .Include(acc => acc.approvedByAccount)
                .Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category).ToList();
            }
        }

        public List<Article> findArticlesGroupByBaseVersion(string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
                {
                    return db.articleDb
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

                else if (SessionPersister.account != null)
                {
                    var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();
                    categories.Add(0);

                    return db.articleDb
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

                return new List<Article>();
            }
        }


        public Article findArticleByID(int articleID)
        {
            using (var db = new BaseDbContext())
            {
                return db.articleDb.Where(acc => acc.ArticleID == articleID)
                    .Include(acc => acc.createdByAccount)
    .Include(acc => acc.approvedByAccount)
    .Include(acc => acc.publishedByAccount)
                    .Include(acc => acc.category).FirstOrDefault();
            }
        }

        public Article findArticleByIDNoTracking(int articleID)
        {
            using (var db = new BaseDbContext())
            {
                return db.articleDb.AsNoTracking().Where(acc => acc.ArticleID == articleID)
                .Include(acc => acc.createdByAccount)
.Include(acc => acc.approvedByAccount)
.Include(acc => acc.publishedByAccount)
                .Include(acc => acc.category).FirstOrDefault();
            }
        }


        public List<Article> findArticlesRequestingApproval()
        {
            using (var db = new BaseDbContext())
            {
                if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
                {
                    return db.articleDb.Where(acc =>
                    acc.isRequestingApproval == true && acc.Lang == "en")
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

                    return db.articleDb.Where(acc =>
                    acc.isRequestingApproval == true && acc.Lang == "en"
                    && categories.Contains(acc.categoryID ?? 0))
                    .OrderByDescending(acc => acc.modified_at)
                    .Include(acc => acc.createdByAccount)
    .Include(acc => acc.approvedByAccount)
    .Include(acc => acc.publishedByAccount)
                    .Include(acc => acc.category)
                    .ToList();
                }

                return new List<Article>();
            }
        }

        public List<Article> findArticlesGroupByBaseVersionRequestingApproval(string lang = "en")
        {
            using (var db = new BaseDbContext())
            {
                return db.articleDb
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
        }

        public List<Article> findArticlesGroupByBaseVersionApproved(int baseArticleID = 0, string lang = "en", BaseDbContext _db = null)
        {
            using (var db = new BaseDbContext())
            {
                var __db = db;

                if (_db != null)
                {
                    __db = _db;
                }

                if (baseArticleID == 0)
                {
                    return __db.articleDb
                        .GroupBy(acc => acc.BaseArticleID)
                        .Select(u => u.Where(acc => acc.Lang == lang && acc.isApproved == true).OrderByDescending(acc => acc.isPublished)
                        .FirstOrDefault())
                        .Where(acc => acc.isApproved == true)
                        .OrderByDescending(acc => acc.dateApproved)
                        .Include(acc => acc.createdByAccount)
        .Include(acc => acc.approvedByAccount)
        .Include(acc => acc.publishedByAccount)
                        .Include(acc => acc.category)
                        .ToList();
                }
                return __db.articleDb
                    .GroupBy(acc => acc.BaseArticleID)
                    .Select(u => u.Where(acc => acc.Lang == lang && acc.isApproved == true && acc.BaseArticleID == baseArticleID).OrderByDescending(acc => acc.isPublished)
                    .FirstOrDefault())
                    .Where(acc => acc.isApproved == true)
                    .OrderByDescending(acc => acc.dateApproved)
                    .Include(acc => acc.createdByAccount)
    .Include(acc => acc.approvedByAccount)
    .Include(acc => acc.publishedByAccount)
                    .Include(acc => acc.category)
                    .ToList();
            }
        }


        public Article findArticleByVersionAndLang(int baseArticleID, int version = 0, String lang = null)
        {
            using (var db = new BaseDbContext())
            {
                if (lang == null)
                {
                    return null;
                }

                if (version == 0)
                {
                    return db.articleDb.Where(acc =>
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
                    return db.articleDb.Where(acc =>
                    acc.BaseArticleID == baseArticleID &&
                    acc.Version == version &&
                    acc.Lang == lang)
                    .Include(acc => acc.createdByAccount)
    .Include(acc => acc.approvedByAccount)
    .Include(acc => acc.publishedByAccount)
                    .Include(acc => acc.category).FirstOrDefault();
                }
            }
        }

        public Article findArticleByArticle(Article article)
        {
            return null;
            //   return articleDb.Where(acc => acc.Username == account.Username && acc.Password == account.Password).FirstOrDefault();
        }


        public Article findLatestArticleByBaseArticle(Article article, String lang = null)
        {
            using (var db = new BaseDbContext())
            {
                var targetBaseArticleID = article.BaseArticleID;
                var targetLang = article.Lang;

                var baseLatestArticle = db.articleDb.Where(acc =>
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
        }


        public Article findLatestArticleByBaseArticleID(int baseArticleID, String lang = null)
        {
            using (var db = new BaseDbContext())
            {
                var targetBaseArticleID = baseArticleID;
                var targetLang = lang;

                var baseLatestArticle = db.articleDb.Where(acc =>
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
        }



        public List<Article> findAllArticlesByBaseArticle(Article article, String lang = null)
        {
            using (var db = new BaseDbContext())
            {
                var articles = db.articleDb.Where(acc =>
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
        }



        public List<Article> findAllArticlesByBaseArticleIncludingAllLocales(Article article)
        {
            using (var db = new BaseDbContext())
            {
                if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
                {
                    var articles = db.articleDb.Where(acc =>
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

                    var articles = db.articleDb.Where(acc =>
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

                return new List<Article>();
            }
        }





        public List<Article> findAllLocaleArticlesByBaseArticleAndVersion(Article article, String Lang = "en", BaseDbContext _db = null)
        {
            using (var db = new BaseDbContext())
            {
                var __db = db;

                if (_db != null)
                {
                    __db = _db;
                }

                var articles = __db.articleDb.Where(acc =>
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
        }



        public bool frozenArticleAlreadyPresents(Article article)
        {
            using (var db = new BaseDbContext())
            {
                var count = db.articleDb.AsNoTracking().Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.isFrozen == true
            )
            .Count();

                return count > 0;
            }
        }



        public bool articleWithSameVersionAndLangAlreadyPresents(Article article)
        {
            return findArticleByVersionAndLang(article.BaseArticleID, article.Version, article.Lang) != null;
        }

        #endregion
        


        // ARTICLE EDITOR ONLY

        #region "Create"

        public String tryCreateNewArticle(Article article)
        {
            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(article);
            if (error != null)
            {
                return error;
            }

            Article latestArticle = null;

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
                article = latestArticle.makeNewArticleByCloningContent();
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

            using (var db = new BaseDbContext())
            {
                article.createdBy = SessionPersister.account.AccountID;
                db.articleDb.Add(article);
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
                    AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_CREATE);
                }
                else
                {
                    AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_CREATE_NEW_VERSION);
                }

            }
            return null;
        }


        void tryCloningNewLocaleArticleForNewArticleVersion(Article latestArticle, Article newArticle)
        {
            using (var db = new BaseDbContext())
            {
                // try clone new locale for this new article
                var articles = findAllLocaleArticlesByBaseArticleAndVersion(latestArticle, latestArticle.Lang);
                foreach (var _a in articles)
                {
                    Article _new = _a.makeNewArticleByCloningContent();
                    _new.Version = newArticle.Version;
                    _new.createdBy = SessionPersister.account.AccountID;
                    db.articleDb.Add(_new);
                }
                db.SaveChanges();
            }
        }


        public String tryCreateNewLocaleArticle(Article article)
        {
            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(article);
            if (error != null)
            {
                return error;
            }

            if (article.BaseArticleID != 0)
            {
                if (article.Version == 0)
                {
                    var latestArticle = findLatestArticleByBaseArticle(article, article.Lang);
                    article.Version = latestArticle.Version;

                    error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(latestArticle);
                    if (error != null)
                    {
                        return error;
                    }
                }
            }

            if (articleWithSameVersionAndLangAlreadyPresents(article))
            {
                return "Article with the same version and language already presents";
            }

            using (var db = new BaseDbContext())
            {
                article.createdBy = SessionPersister.account.AccountID;
                db.articleDb.Add(article);
                db.SaveChanges();


                if (article.BaseArticleID == 0)
                {
                    db.Entry(article).State = EntityState.Modified;
                    article.BaseArticleID = article.ArticleID;
                    db.SaveChanges();
                }
            }

            return null;
        }

        #endregion

        #region "Edit"

        public String tryEditArticle(Article article)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                if (!ConstantDbContext.getInstance().ALLOW_EDIT_AFTER_PUBLISH())
                {
                    return ResHelper.S("itemisfrozen");
                }
            }

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            using (var db = new BaseDbContext())
            {
                db.Entry(_article).State = EntityState.Modified;
                _article.Name = article.Name;
                _article.Desc = article.Desc;
                _article.Slug = article.Slug;
                _article.Keywords = article.Keywords;
                _article.MetaData = article.MetaData;
                _article.MetaKeywords = article.MetaKeywords;
                _article.Excerpt = article.Excerpt;
                db.SaveChanges();

                AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_EDIT);

                return null;
            }
        }



        public String tryEditArticleProperties(Article article, bool allLocales)
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
                if (!ConstantDbContext.getInstance().ALLOW_EDIT_AFTER_PUBLISH())
                {
                    return ResHelper.S("itemisfrozen");
                }
            }

            var error = AccountGroupBaseArticlePermissionHelper.tryCatchAccountGroupPermissionError(_article);
            if (error != null)
            {
                return error;
            }

            using (var db = new BaseDbContext())
            {
                db.Entry(_article).State = EntityState.Modified;
                _article.Url = article.Url;
                _article.Slug = article.Slug;
                _article.categoryID = article.categoryID;

                if (allLocales)
                {
                    var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang, db);
                    foreach (var _a in _localeArticles)
                    {
                        db.Entry(_a).State = EntityState.Modified;
                        _a.Url = article.Url;
                        _a.Slug = article.Slug;
                        _a.categoryID = article.categoryID;
                    }
                }

                db.SaveChanges();

                AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_EDIT_PROPERTIES);

                return null;
            }
        }

        #endregion

        #region "Delete"


        public String tryDeleteArticle(Article article, bool isRecursive)
        {
            using (var db = new BaseDbContext())
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

                AuditLogDbContext.getInstance().createAuditLogArticleAction(_article, AuditLogDbContext.ACTION_DELETE);

                if (isRecursive)
                {
                    if (frozenArticleAlreadyPresents(_article))
                    {
                        error = ResHelper.S("itemisfrozen");
                        return error;
                    }
                    db.articleDb.RemoveRange(db.articleDb.Where((acc) => acc.BaseArticleID == _article.BaseArticleID));
                }
                else
                {
                    db.articleDb.Remove(article);
                }
                db.SaveChanges();

                return null;
            }
        }

        #endregion




        // ARTICLE EDITOR & APPROVER ONLY

        #region "Approval"

        // ARTICLE EDITOR REQUEST FOR APPROVAL

        public String trySubmitRequestForApproval(Article article, bool allLocales)
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

            using (var db = new BaseDbContext())
            {
                db.Entry(_article).State = EntityState.Modified;
                _article.isRequestingApproval = true;
                _article.isFrozen = true;

                if (allLocales)
                {
                    var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang, db);
                    foreach (var _a in _localeArticles)
                    {
                        db.Entry(_a).State = EntityState.Modified;
                        _article.isRequestingApproval = true;
                        _article.isFrozen = true;
                    }
                }

                db.SaveChanges();

                AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_SUBMIT_FOR_APPROVAL);

                return null;
            }
        }




        // ARTICLE APPROVER ONLY

        public String tryRequestApproval(Article article, bool allLocales)
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

            using (var db = new BaseDbContext())
            {
                db.Entry(_article).State = EntityState.Modified;
                _article.isApproved = true;
                _article.isUnapproved = false;
                _article.isFrozen = true;
                _article.dateApproved = DateTime.UtcNow;
                _article.approvalRemarks = article.approvalRemarks;
                _article.approvedBy = SessionPersister.account.AccountID;

                if (allLocales)
                {
                    var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang, db);
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

                AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_APPROVE);

                return null;
            }
        }

        public String tryRequestUnapproval(Article article, bool allLocales)
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

            using (var db = new BaseDbContext())
            {
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
                    var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang, db);
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

                AuditLogDbContext.getInstance().createAuditLogArticleAction(article, AuditLogDbContext.ACTION_UNAPPROVE);

                return null;
            }
        }

        #endregion
    }
}