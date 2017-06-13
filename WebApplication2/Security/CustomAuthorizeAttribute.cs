using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Controllers;
using WebApplication2.Helpers;
using WebApplication2.Models;

namespace WebApplication2.Security
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If loggedin , stop going to log in page
            if (SessionPersister.account != null)
            {
                if ((filterContext.Controller is AccountController)
                    && (
                    filterContext.ActionDescriptor.ActionName == "Login"
                    ))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Account", action = "Index" }));
                    return;
                }
            }

            // Not logged in check
            if (SessionPersister.account == null)
            {
                if (!(filterContext.Controller is AccountController)
                    || (
                    filterContext.ActionDescriptor.ActionName != "Login"
                    ))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Account", action = "Login" }));
                    return;
                }
                else
                {
                    SessionPersister.removeSession();
                    return;
                }
            }


            // 30 min idle check
            Account account = SessionPersister.account;
            DateTime? accountLastActivity = SessionPersister.account_last_activity;

            bool isLastActivityExpired = false;

            if (accountLastActivity.GetValueOrDefault() != null)
            {
                int sessionMin = 30;

                Constant sessionMinConst = ConstantDbContext.getInstance().findActiveByKeyNoTracking("CMS_SESSION_KEEPALIVE_MINS");
                if (sessionMinConst != null && sessionMinConst.Value != null)
                {
                    int _sessionMin = int.Parse(sessionMinConst.Value);
                    if (_sessionMin >= 1)
                    {
                        sessionMin = _sessionMin;
                    }
                }

                if ((DateTimeExtensions.GetServerTime() - accountLastActivity.GetValueOrDefault()).TotalMinutes > sessionMin)
                {
                    isLastActivityExpired = true;
                }
            }

            if (SessionPersister.account == null)
            {
                if (isLastActivityExpired)
            {
                SessionPersister.removeSession();
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Account", action = "Expired" }));
                return;
            }
            else
            {
                SessionPersister.refresh_account_last_activity();
            }
            }



            // Role check
            CustomPrincipal mp = new CustomPrincipal(account);
            if (!mp.IsInRole(Roles))
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "AccessDenied", action = "Index" }));
                return;
            }



            // Password Change Request after 90 day' Last Password Modified At
            // or Needchangepassword == true
            var needchangepassword = false;
            if (account.LastPasswordModifiedAt.GetValueOrDefault() != null)
            {
                if ((DateTimeExtensions.GetServerTime() - account.LastPasswordModifiedAt.GetValueOrDefault()).TotalDays > 90)
                {
                    needchangepassword = true;
                }
            }
            if (account.NeedChangePassword)
            {
                needchangepassword = true;
            }

            if (needchangepassword)
            {
                if (!(filterContext.Controller is AccountController)
                    || (
                    filterContext.ActionDescriptor.ActionName != "ChangePassword" &&
                    filterContext.ActionDescriptor.ActionName != "ChangePasswordSuccess"
                    ))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Account", action = "ChangePassword" }));
                    return;
                }
            }

        }
    }
}