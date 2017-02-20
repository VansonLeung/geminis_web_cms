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

            
            if (SessionTimeout())
            {
                ClearSession();

                category = "login";
                id = null;
                BaseViewModel vml = BaseViewModel.make(locale, category, id, Request, getSession());
                return View(vml);
            }

            InternalKeepAlive();

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