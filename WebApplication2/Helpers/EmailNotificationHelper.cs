using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using WebApplication2.Context;
using WebApplication2.Models;
using WebApplication2.Models.Infrastructure;
using WebApplication2.Security;

namespace WebApplication2.Helpers
{
    public class EmailNotificationHelper
    {
        public enum EmailNotificationAction
        {
            UNKNOWN,
            CREATE,
            EDIT,
            EDITPROPERTIES,
            DELETE,
            CREATENEWVERSION,
            SUBMITFORAPPROVAL,
            APPROVE,
            UNAPPROVE,
            PUBLISH,
            UNPUBLISH
        }

        public static EmailNotificationAction ParseAction(string action)
        {
            EmailNotificationHelper.EmailNotificationAction notificationAction = EmailNotificationHelper.EmailNotificationAction.UNKNOWN;

            if (action.Equals(AuditLogDbContext.ACTION_CREATE))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.CREATE;
            }
            if (action.Equals(AuditLogDbContext.ACTION_EDIT))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.EDIT;
            }
            if (action.Equals(AuditLogDbContext.ACTION_EDIT_PROPERTIES))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.EDITPROPERTIES;
            }
            if (action.Equals(AuditLogDbContext.ACTION_DELETE))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.DELETE;
            }
            if (action.Equals(AuditLogDbContext.ACTION_CREATE_NEW_VERSION))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.CREATENEWVERSION;
            }
            if (action.Equals(AuditLogDbContext.ACTION_SUBMIT_FOR_APPROVAL))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.SUBMITFORAPPROVAL;
            }
            if (action.Equals(AuditLogDbContext.ACTION_APPROVE))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.APPROVE;
            }
            if (action.Equals(AuditLogDbContext.ACTION_UNAPPROVE))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.UNAPPROVE;
            }
            if (action.Equals(AuditLogDbContext.ACTION_PUBLISH))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.PUBLISH;
            }
            if (action.Equals(AuditLogDbContext.ACTION_UNPUBLISH))
            {
                notificationAction = EmailNotificationHelper.EmailNotificationAction.UNPUBLISH;
            }

            return notificationAction;
        }

        public static bool NotifyAllOnActionOfBaseArticle(string categoryStr, BaseArticle baseArticle, EmailNotificationAction action, List<Object> parameters = null)
        {
            if (action == EmailNotificationAction.UNKNOWN)
            {
                return false;
            }

            var categoryID = baseArticle.categoryID;
            Category category = null;

            if (baseArticle.categoryID == null || baseArticle.categoryID <= 0)
            {
                category = InfrastructureCategoryDbContext.getInstance().findCategoryByID(categoryID);
            }




            // get createdBy
            Account owner = AccountDbContext.getInstance().findAccountByID(SessionPersister.account.AccountID);



            // analyze category for interested accounts (by account group)
            List<AccountGroup> accountGroups = AccountGroupDbContext.getInstance().findGroups();
            List<AccountGroup> filteredAccountGroups = new List<AccountGroup>();

            foreach (var group in accountGroups)
            {
                List<int> categoryIDs = group.getAccessibleCategoryListInt();
                if (category != null && categoryIDs.Contains(category.ItemID))
                {
                    filteredAccountGroups.Add(group);
                }
            }


            List<Account> accounts = AccountDbContext.getInstance().findAccountsByAccountGroupsToEmailNotify(filteredAccountGroups, baseArticle);

            // filter...
            List<Account> filteredAccounts = new List<Account>();

            if (action == EmailNotificationAction.CREATE
                || action == EmailNotificationAction.EDIT
                || action == EmailNotificationAction.EDITPROPERTIES
                || action == EmailNotificationAction.CREATENEWVERSION)
            {
                foreach (var acc in accounts)
                {
                    if (acc.isRoleSuperadmin() || acc.isRoleEditor())
                    {
                        filteredAccounts.Add(acc);
                    }
                }
            }
            if (action == EmailNotificationAction.DELETE)
            {
                foreach (var acc in accounts)
                {
                    if (acc.isRoleSuperadmin() || acc.isRoleEditor())
                    {
                        filteredAccounts.Add(acc);
                    }
                }
            }

            if (action == EmailNotificationAction.SUBMITFORAPPROVAL)
            {
                foreach (var acc in accounts)
                {
                    if (acc.isRoleSuperadmin() || acc.isRoleEditor() || acc.isRoleApprover())
                    {
                        filteredAccounts.Add(acc);
                    }
                }
            }

            if (action == EmailNotificationAction.APPROVE
                || action == EmailNotificationAction.UNAPPROVE)
            {
                foreach (var acc in accounts)
                {
                    if (acc.isRoleSuperadmin() || acc.isRoleEditor() || acc.isRoleApprover() || acc.isRolePublisher())
                    {
                        filteredAccounts.Add(acc);
                    }
                }
            }

            if (action == EmailNotificationAction.PUBLISH
                || action == EmailNotificationAction.UNPUBLISH)
            {
                foreach (var acc in accounts)
                {
                    if (acc.isRoleSuperadmin() || acc.isRoleEditor() || acc.isRoleApprover() || acc.isRolePublisher())
                    {
                        filteredAccounts.Add(acc);
                    }
                }
            }

            foreach (var acc in filteredAccounts)
            {
                // send to owner?
                if (owner != null && owner.AccountID == acc.AccountID 
                    && owner.ShouldEmailNotifyBaseArticleChangesByOwn())
                {
                    SendEmail(categoryStr, owner, acc, baseArticle, category, action);
                    continue;
                }

                // send to all?
                if (acc.ShouldEmailNotifyBaseArticleChanges())
                {
                    SendEmail(categoryStr, owner, acc, baseArticle, category, action);
                    continue;
                }
            }

            return true;
        }



        public static System.Collections.Generic.Dictionary<string, string> MakeNotificationInfo(string categoryStr, Account owner, Account account, BaseArticle baseArticle, Category category, EmailNotificationAction action)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            var item_cat = "Item";
            var item_url = "";
            var articleID = baseArticle.ArticleID;
            var baseArticleID = baseArticle.BaseArticleID;
            var name = baseArticle.Name;

            if (categoryStr.Equals("Article"))
            {
                item_cat = "Article";
                if (account.isRolePublisher())
                {
                    item_url = "/ArticlePublisher/DetailsLocale?baseArticleID=" + baseArticle.BaseArticleID + "&version=" + baseArticle.Version + "&lang=" + baseArticle.Lang;
                }
                else if (account.isRoleApprover())
                {
                    item_url = "/ArticleApprover/DetailsLocale?baseArticleID=" + baseArticle.BaseArticleID + "&version=" + baseArticle.Version + "&lang=" + baseArticle.Lang;
                }
                else if (account.isRoleEditor())
                {
                    item_url = "/ArticleEditor/DetailsLocale?baseArticleID=" + baseArticle.BaseArticleID + "&version=" + baseArticle.Version + "&lang=" + baseArticle.Lang;
                }
            }
            else if (categoryStr.Equals("Content Page"))
            {
                item_cat = "Content Page";
                item_url = "/ContentPageEditor/DetailsLocale?baseArticleID=" + baseArticle.BaseArticleID + "&version=" + baseArticle.Version + "&lang=" + baseArticle.Lang;
            }

            item_url = ServerHelper.GetSiteRoot() + item_url;

            var item_action_tag = "";
            var item_action_description = "";
            switch (action)
            {
                case EmailNotificationAction.CREATE:
                    item_action_tag = "Created";
                    item_action_description = "{0} {1} has been created by {2}.";
                    break;
                case EmailNotificationAction.EDIT:
                    item_action_tag = "Edited";
                    item_action_description = "{0} {1}'s contents has been edited by {2}.";
                    break;
                case EmailNotificationAction.EDITPROPERTIES:
                    item_action_tag = "Properties Edited";
                    item_action_description = "{0} {1}'s properties has been edited by {2}.";
                    break;
                case EmailNotificationAction.DELETE:
                    item_action_tag = "Deleted";
                    item_action_description = "{0} {1} has been deleted by {2}.";
                    break;
                case EmailNotificationAction.CREATENEWVERSION:
                    item_action_tag = "Created New Version";
                    item_action_description = "A new version of {0} {1} has been created by {2}.";
                    break;
                case EmailNotificationAction.SUBMITFORAPPROVAL:
                    item_action_tag = "Submitted for Approval";
                    item_action_description = "{0} {1} has been submitted for approval by {2}.";
                    break;
                case EmailNotificationAction.APPROVE:
                    item_action_tag = "Approved";
                    item_action_description = "{0} {1} has been approved by {2}.";
                    break;
                case EmailNotificationAction.UNAPPROVE:
                    item_action_tag = "Unapproved";
                    item_action_description = "{0} {1} has been unapproved by {2}.";
                    break;
                case EmailNotificationAction.PUBLISH:
                    item_action_tag = "Published";
                    item_action_description = "{0} {1} has been published by {2}.";
                    break;
                case EmailNotificationAction.UNPUBLISH:
                    item_action_tag = "Unpublished";
                    item_action_description = "{0} {1} has been unpublished by {2}.";
                    break;
                default:
                    break;
            }

            var item_subject = String.Format("[GSL - {0} {1}] ({2}) {3}",
                item_cat,
                baseArticleID,
                item_action_tag,
                name
            );


            var item_action_description_impl = string.Format(
                item_action_description,
                item_cat,
                baseArticleID,
                owner.Username
            );


            var item_body = string.Format(
                "Dear {0} {1}, <br/><br/>" +
                "<p>{2}</p>" +
                "<p><a href='{3}'>{4}</a></p>" +
                "<hr />" +
                "<p>Geminis CMS Team</p>",
                account.Firstname,
                account.Lastname,
                item_action_description_impl,
                item_url,
                item_subject
            );

            dict.Add("subject", item_subject);
            dict.Add("body", item_body);

            return dict;
        }
        

        public static void SendEmail(string categoryStr, Account owner, Account account, BaseArticle baseArticle, Category category, EmailNotificationAction action)
        {
            var dict = MakeNotificationInfo(categoryStr, owner, account, baseArticle, category, action);
            var body = dict["body"];
            var subject = dict["subject"];

            Thread email = new Thread(delegate ()
            {
                EmailHelper.SendEmail(
                    new List<string> { account.Email },
                    body,
                    subject
                );
            });
            
            email.IsBackground = true;
            email.Start();
        }

    }
}