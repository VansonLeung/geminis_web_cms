using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frontend.Controllers;
using WebApplication2.Models;

namespace Frontend.Security
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Not logged in check
            if (SessionPersister.account == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "User", action = "Login" }));
                return;
            }


            // 30 min idle check
            User account = SessionPersister.account;
            DateTime? accountLastActivity = SessionPersister.account_last_activity;

            bool isLastActivityExpired = false;

            if (accountLastActivity.GetValueOrDefault() != null)
            {
                if ((DateTime.UtcNow - accountLastActivity.GetValueOrDefault()).TotalMinutes > 30)
                {
                    isLastActivityExpired = true;
                }
            }

            if (isLastActivityExpired)
            {
                SessionPersister.removeSession();
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "User", action = "Expired" }));
                return;
            }
            else
            {
                SessionPersister.refresh_account_last_activity();
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
            /*
            var needchangepassword = false;
            if (account.LastPasswordModifiedAt.GetValueOrDefault() != null)
            {
                if ((DateTime.UtcNow - account.LastPasswordModifiedAt.GetValueOrDefault()).TotalDays > 90)
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
                if (!(filterContext.Controller is UserController)
                    || (
                    filterContext.ActionDescriptor.ActionName != "ChangePassword" &&
                    filterContext.ActionDescriptor.ActionName != "ChangePasswordRequest" &&
                    filterContext.ActionDescriptor.ActionName != "ChangePasswordSuccess"
                    ))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "User", action = "ChangePasswordRequest" }));
                    return;
                }
            }
            */
        }
    }
}