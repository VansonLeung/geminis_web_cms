using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Helpers
{
    public class AccountGroupBaseArticlePermissionHelper
    {
        public static bool articleIsPermittedUnderAccountGroup(BaseArticle article)
        {
            var account = SessionPersister.account;
            if (account != null)
            {
                if (account.isRoleSuperadmin())
                {
                    return true;
                }

                if (account.Group != null)
                {
                    if (article.categoryID == null)
                    {
                        return true;
                    }
                    var categoryID = string.Format("{0}", article.categoryID);
                    if (account.Group.getAccessibleCategoryList().Contains(categoryID))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string tryCatchAccountGroupReadPermissionError(BaseArticle article)
        {
            if (!articleIsPermittedUnderAccountGroup(article))
            {
                return "Access Denied";
            }
            return null;
        }
        public static string tryCatchAccountGroupPermissionError(BaseArticle article)
        {
            if (!articleIsPermittedUnderAccountGroup(article))
            {
                return "Access Denied";
            }
            return null;
        }
    }
}