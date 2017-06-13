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
        public static string ACTION_CHANGE_PASSWORD = "CHANGE_PASSWORD";
        public static string ACTION_ACTIVATE = "ACTIVATE";
        public static string ACTION_DEACTIVATE = "DEACTIVATE";

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
        

        #endregion




        #region "Query"

        public class Query
        {
            public string accountID = "";
            public string logAction = "";
            public string startDate = "";
            public string endDate = "";
            public string category = "";
            public string article = "";
            public bool is_private = false;

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
            using (var db = new BaseDbContext())
            {
                var itemDb = db.auditLogDb;

                var predicate = PredicateBuilder.True<AuditLog>();

                if (!String.IsNullOrEmpty(query.accountID) && query.accountID != "0")
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
                if (!String.IsNullOrEmpty(query.category))
                {
                    String category = query.category;
                    predicate = predicate.And(acc => acc.category == category);
                }
                if (!String.IsNullOrEmpty(query.article))
                {
                    String article = query.article;
                    predicate = predicate.And(acc => acc.article == article);
                }

                predicate = predicate.And(acc => acc.is_private == query.is_private);

                return itemDb.AsExpandable()
                    .Where(predicate)
                    .OrderByDescending(item => item.modified_at)
                    .ToList();
            }
        }

        public AuditLog findByID(int ID)
        {
            using (var db = new BaseDbContext())
            {
                return db.auditLogDb
                .Where(item => item.logID == ID)
                .FirstOrDefault();
            }
        }

        #endregion


        #region "Create"

        // edit

        void manipulateRemarks(AuditLog item, List<string> modified_fields)
        {
            if (modified_fields == null)
            {
                modified_fields = new List<string>();
            }

            var unique_items = new HashSet<string>(modified_fields);

            if (unique_items.Count > 0)
            {
                item.remarks = string.Join(",", unique_items);
            }
        }

        public string createAuditLog(AuditLog item)
        {
            using (var db = new BaseDbContext())
            {
                db.auditLogDb.Add(item);
                db.SaveChanges();

                return null;
            }
        }

        public string createAuditLogArticleAction(Article article, string action, List<string> modified_fields = null)
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

            manipulateRemarks(item, modified_fields);

            return createAuditLog(item);
        }
        public string createAuditLogContentPageAction(ContentPage contentPage, string action, List<string> modified_fields = null)
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

            manipulateRemarks(item, modified_fields);

            return createAuditLog(item);
        }
        public string createAuditLogCategoryAction(Category category, string action, List<string> modified_fields = null)
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

            manipulateRemarks(item, modified_fields);

            return createAuditLog(item);
        }


        public string createAuditLogAccountAction(Account targetAccount, string action, List<string> modified_fields = null)
        {

            var account = SessionPersister.account;
            if (account == null)
            {
                return null;
            }

            AuditLog item = new AuditLog();
            item.accountID = account.AccountID;
            item.account = account.Username;
            item.targetAccountID = targetAccount.AccountID;
            item.targetAccount = targetAccount.Username;
            item.action = action;

            manipulateRemarks(item, modified_fields);

            return createAuditLog(item);
        }


        public string createAuditLogSystemMaintenanceNotificationAction(SystemMaintenanceNotification systemMaintenanceNotification, string action, List<string> modified_fields = null)
        {

            var account = SessionPersister.account;
            if (account == null)
            {
                return null;
            }

            AuditLog item = new AuditLog();
            item.accountID = account.AccountID;
            item.account = account.Username;
            item.systemMaintenanceNotificationID = systemMaintenanceNotification.NotificationID;
            item.systemMaintenanceNotification = systemMaintenanceNotification.name_en;
            item.action = action;

            manipulateRemarks(item, modified_fields);

            return createAuditLog(item);
        }


        public string createAuditLogConstantAction(Constant constant, string action, List<string> modified_fields = null)
        {

            var account = SessionPersister.account;
            if (account == null)
            {
                return null;
            }

            AuditLog item = new AuditLog();
            item.accountID = account.AccountID;
            item.account = account.Username;
            item.article = constant.Key;
            item.action = action;

            manipulateRemarks(item, modified_fields);

            return createAuditLog(item);
        }


        #endregion
    }
}