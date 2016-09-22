using LinqKit.Core;
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
    public class AuditLogDbContext
    {
        #region "Constants"

        public static string ACTION_CREATE = "CREATE";
        public static string ACTION_EDIT = "EDIT";
        public static string ACTION_EDIT_PROPERTIES = "EDIT_PROPERTIES";
        public static string ACTION_DELETE = "DELETE";
        public static string ACTION_CREATE_NEW_VERSION = "CREATE_NEW_VERSION";
        public static string ACTION_SUBMIT_FOR_APPROVAL = "SUBMIT_FOR_APPROVAL";
        public static string ACTION_APPROVE = "APPROVE";
        public static string ACTION_UNAPPROVE = "UNAPPROVE";
        public static string ACTION_PUBLISH = "PUBLISH";
        public static string ACTION_UNPUBLISH = "UNPUBLISH";

        #endregion



        #region "Constructor"

        // singleton

        private static AuditLogDbContext instance;

        public static AuditLogDbContext getInstance()
        {
            if (instance == null)
            {
                instance = new AuditLogDbContext();
            }
            return instance;
        }


        // initializations 

        private BaseDbContext db = BaseDbContext.getInstance();

        protected virtual DbSet<AuditLog> getItemDb()
        {
            return db.auditLogDb;
        }

        #endregion




        #region "Query"

        public class Query
        {
            public string accountID = "";
            public string logAction = "";
            public string startDate = "";
            public string endDate = "";

            public int getAccountID()
            {
                return Convert.ToInt32(accountID);
            }

            public DateTime getStartDate()
            {
                return DateTimeExtensions.StringToDateTime(startDate);
            }
            public DateTime getEndDate()
            {
                return DateTimeExtensions.StringToDateTime(endDate);
            }
        }

        
        // query

        public List<AuditLog> findAll(Query query)
        {
            var itemDb = getItemDb();

            var predicate = PredicateBuilder.True<AuditLog>();

            if (!String.IsNullOrEmpty(query.accountID))
            {
                int id = query.getAccountID();
                predicate = predicate.And(acc => acc.accountID == id);
            }
            if (!String.IsNullOrEmpty(query.logAction))
            {
                predicate = predicate.And(acc => acc.action == query.logAction);
            }
            if (!String.IsNullOrEmpty(query.startDate))
            {
                DateTime date = query.getStartDate();
                predicate = predicate.And(acc => acc.created_at >= date);
            }
            if (!String.IsNullOrEmpty(query.endDate))
            {
                DateTime date = query.getEndDate();
                predicate = predicate.And(acc => acc.created_at <= date);
            }


            return itemDb.AsExpandable()
                .Where(predicate)
                .OrderByDescending(item => item.modified_at)
                .ToList();
        }

        public AuditLog findByID(int ID)
        {
            return getItemDb()
                .Where(item => item.logID == ID)
                .FirstOrDefault();
        }

        #endregion


        #region "Create"

        // edit

        public string createAuditLog(AuditLog item)
        {
            getItemDb().Add(item);
            db.SaveChanges();

            return null;
        }

        public string createAuditLogArticleAction(Article article, string action)
        {
            var _article = ArticleDbContext.getInstance().findArticleByIDNoTracking(article.ArticleID);
            if (_article == null)
            {
                return null;
            }

            var account = SessionPersister.account;
            if (account == null)
            {
                return null;
            }

            var notificationAction = EmailNotificationHelper.ParseAction(action);
            EmailNotificationHelper.NotifyAllOnActionOfBaseArticle("Article", _article, notificationAction);
            
            AuditLog item = new AuditLog();
            item.accountID = account.AccountID;
            item.account = account.Username;
            item.articleID = _article.ArticleID;
            item.article = _article.Name;
            item.categoryID = _article.categoryID;
            item.category = _article.category != null ? _article.category.GetName() : null;
            item.action = action;

            return createAuditLog(item);
        }
        public string createAuditLogContentPageAction(ContentPage contentPage, string action)
        {
            var _contentPage = ContentPageDbContext.getInstance().findArticleByIDNoTracking(contentPage.ArticleID);
            if (_contentPage == null)
            {
                return null;
            }

            var account = SessionPersister.account;
            if (account == null)
            {
                return null;
            }

            var notificationAction = EmailNotificationHelper.ParseAction(action);
            EmailNotificationHelper.NotifyAllOnActionOfBaseArticle("Content Page", contentPage, notificationAction);

            AuditLog item = new AuditLog();
            item.accountID = account.AccountID;
            item.account = account.Username;
            item.contentPageID = _contentPage.ArticleID;
            item.contentPage = _contentPage.Name;
            item.categoryID = _contentPage.categoryID;
            item.category = _contentPage.category != null ? _contentPage.category.GetName() : null;
            item.action = action;

            return createAuditLog(item);
        }
        public string createAuditLogCategoryAction(Category category, string action)
        {
            var account = SessionPersister.account;
            if (account == null)
            {
                return null;
            }

            AuditLog item = new AuditLog();
            item.accountID = account.AccountID;
            item.account = account.Username;
            item.categoryID = category.ItemID;
            item.category = category.GetName();
            item.action = action;

            return createAuditLog(item);
        }


        #endregion
    }
}