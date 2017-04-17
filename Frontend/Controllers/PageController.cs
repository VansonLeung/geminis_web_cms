using Frontend.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.ViewModels.Include;

namespace Frontend.Controllers
{
    public class PageController : BaseController
    {
        [Internationalization]
        public ActionResult Index(string locale, string category, string id)
        {
            /*
            List<WebApplication2.Models.Article> articles = WebApplication2.Context.ArticleDbContext.getInstance().findArticles();
            if (articles.Count > 0)
            {
                log4net.ILog logger = log4net.LogManager.GetLogger("Logger");
                logger.Debug("Can fetch information from CMS");
            }
            */

            
            // check session if timeout

            
            if (SSO_SessionTimeout())
            {
                SSO_ClearSession();

                category = "login";
                id = null;
                BaseViewModel vml = BaseViewModel.make(locale, category, id, Request, getSession());
                ViewBag.message = "Session Expired";
                return View(vml);
            }

            SSO_InternalKeepAlive();
            SSO_InternalHeartbeat();

            if (category != null && category.ToLower() == "home")
            {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index",
                    locale = locale
                });
            }

            BaseViewModel vm = BaseViewModel.make(locale, category, id, Request, getSession());
            return View(vm);
        }
        public ActionResult _Header()
        {
            return PartialView();
        }

        public ActionResult _Footer()
        {
            return PartialView();
        }
    }
}