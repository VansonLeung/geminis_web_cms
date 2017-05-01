using Frontend.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.ViewModels.Include;
using static Frontend.Controllers.SessionController;

namespace Frontend.Controllers
{
    public class HomeController : BaseController
    {
        [Internationalization]
        public ActionResult Index(string locale)
        {
            var keys = Request.QueryString.Keys;
            for (var i = 0; i < keys.Count; i++)
            {
                var val = Request.QueryString[keys[i]];
                if (keys[i] == "lang" && val != null && val != "")
                {
                    locale = val;
                }
            }

            // check session if timeout


            if (SSO_SessionTimeout())
            {
                SSO_ClearSession();

                string category = "login";
                string id = null;
                BaseViewModel vml = BaseViewModel.make(locale, category, id, Request, getSession(true));

                var min = new SessionLogin().getSessionKeepaliveMinutes();

                if (locale == "zh_HK" || locale == "zh_TW")
                {
                    ViewBag.message = "登入時間以空置了超過" + min + "分鐘，請重新登入";
                }
                if (locale == "zh_CN")
                {
                    ViewBag.message = "登入时间以空置了超过" + min + "分钟，请重新登入";
                }
                if (locale == "en")
                {
                    ViewBag.message = "Session has been idled over " + min + " mins, please login again";
                }

                return View(vml);
            }

            SSO_InternalKeepAlive();
            SSO_InternalHeartbeat();

            BaseViewModel vm = BaseViewModel.make(locale, "home", null, Request, getSession());
            return View(vm);
        }


        public ActionResult logout(string locale)
        {
            SSO_ClearSession();
            return Redirect("/" + locale);
        }

        public ActionResult changeFontSizeNormal(string redirect)
        {
            SetFontSizeNormal();
            return Redirect(redirect);
        }

        public ActionResult changeFontSizeBig(string redirect)
        {
            SetFontSizeBig();
            return Redirect(redirect);
        }

        [Internationalization]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Internationalization]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        
        public ActionResult ArticleList()
        {
            var list = ArticleDbContext.getInstance().findArticlesGroupByBaseVersion();
            return PartialView(list);
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