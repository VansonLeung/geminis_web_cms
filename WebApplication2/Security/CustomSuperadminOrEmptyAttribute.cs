using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Controllers;
using WebApplication2.Models;

namespace WebApplication2.Security
{
    public class CustomSuperadminOrEmptyAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // if not have at least 1 superadmin, allow for everyone
            if (!AccountDbContext.getInstance().isSuperadminExists())
            {
                return;
            }

            // if not logged in, redirect login
            if (SessionPersister.account == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Account", action = "Login" }));
                return;
            }

            // if logged in not superadmin, redirect access denied
            if (!SessionPersister.account.Role.Equals("superadmin"))
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "AccessDenied", action = "Index" }));
                return;
            }

            return;
        }
    }
}