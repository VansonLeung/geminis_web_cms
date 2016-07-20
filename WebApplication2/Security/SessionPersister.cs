using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Security
{
    public static class SessionPersister
    {
        static string SESSION_ACCOUNT = "SESSION_ACCOUNT";
        static string SESSION_ACCOUNT_LAST_ACTIVITY = "SESSION_ACCOUNT_LAST_ACTIVITY";

        public static object getSessionByKey(string key)
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            var sessionVar = HttpContext.Current.Session[key];
            if (sessionVar != null)
            {
                return sessionVar;
            }
            return null;
        }

        public static void setSessionByKey(string key, object obj)
        {
            HttpContext.Current.Session[key] = obj;
        }





        public static void createSessionForAccount(Account _account)
        {
            account = _account;
            refresh_account_last_activity();
        }

        public static void removeSession()
        {
            account = null;

        }

        public static Account account
        {
            get
            {
                var obj = getSessionByKey(SESSION_ACCOUNT);
                if (obj != null)
                {
                    return obj as Account;
                }
                return null;
            }
            set
            {
                setSessionByKey(SESSION_ACCOUNT, value);
            }
        }

        public static DateTime? account_last_activity {
            get
            {
                var obj = getSessionByKey(SESSION_ACCOUNT_LAST_ACTIVITY);
                if (obj != null)
                {
                    return obj as DateTime?;
                }
                return null;
            }
            set
            {
                setSessionByKey(SESSION_ACCOUNT_LAST_ACTIVITY, value);
            }
        }

        public static void refresh_account_last_activity()
        {
            account_last_activity = DateTime.UtcNow;
        }
    }
}