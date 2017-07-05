using Frontend.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Helpers;
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
                return Redirect("/" + locale + "/Page/" + "session_timeout");
                /*
                SSO_ClearSession();

                string category = "login";
                string id = null;
                BaseViewModel vml = BaseViewModel.make(locale, category, id, Request, getSession(true));

                var min = new SessionLogin().getSessionKeepaliveMinutes();

                if (locale == "zh-HK" || locale == "zh-TW" || locale == "zh")
                {
                    ViewBag.message = "登入時間以空置了超過" + min + "分鐘，請重新登入";
                }
                if (locale == "zh-CN" || locale == "cn")
                {
                    ViewBag.message = "登入时间以空置了超过" + min + "分钟，请重新登入";
                }
                else
                {
                    ViewBag.message = "Session has been idled over " + min + " mins, please login again";
                }

                if (locale != null)
                {
                    Session["LANG"] = locale;
                }
                */
            }

            SSO_InternalKeepAlive();
            SSO_InternalHeartbeat();

            var session = getSession();
            if (session != null && !session.isKeptAlive)
            {
                Session["isKeptAlive"] = true;

                if (session.hasTradingAcc)
                {
                    return Redirect("/"+locale+"/Page/trading");
                }
            }

            BaseViewModel vm = BaseViewModel.make(locale, "home", null, Request, session);

            if (locale != null)
            {
                Session["LANG"] = locale;
            }
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









        [Internationalization]
        public ActionResult Sitemap(string locale = "en-US")
        {
            // check session if timeout
            if (SSO_SessionTimeout())
            {
                SSO_ClearSession();
            }

            SSO_InternalKeepAlive();
            SSO_InternalHeartbeat();

            var session = getSession();
            if (session != null && !session.isKeptAlive)
            {
                Session["isKeptAlive"] = true;
            }

            BaseViewModel vm2 = BaseViewModel.make(locale, "home", null, Request, session);

            if (locale != null)
            {
                Session["LANG"] = locale;
            }
            return View(vm2);
        }




        [Internationalization]
        public ActionResult Search(string locale = "en-US")
        {
            // check session if timeout
            if (SSO_SessionTimeout())
            {
                SSO_ClearSession();
            }

            SSO_InternalKeepAlive();
            SSO_InternalHeartbeat();

            var session = getSession();
            if (session != null && !session.isKeptAlive)
            {
                Session["isKeptAlive"] = true;
            }

            var keys = Request.QueryString.Keys;
            for (var i = 0; i < keys.Count; i++)
            {
                var val = Request.QueryString[keys[i]];
                if (keys[i] == "q" && val != null && val != "")
                {
                    List<LuceneSearchData> searchData = LuceneSearch.Search(val).ToList();

                    foreach (LuceneSearchData data in searchData)
                    {
                        data.Name = data.GetName(locale);
                        data.Description = data.GetDesc(locale);
                        data.Url = data.GetURL(locale);
                    }

                    BaseViewModel vm = BaseViewModel.make(locale, "home", null, Request, session);
                    vm.search_keywords = val;
                    vm.search_data = searchData;

                    if (locale != null)
                    {
                        Session["LANG"] = locale;
                    }
                    return View(vm);
                }
            }
            BaseViewModel vm2 = BaseViewModel.make(locale, "home", null, Request, session);
            vm2.search_keywords = "";
            vm2.search_data = new List<LuceneSearchData>();

            if (locale != null)
            {
                Session["LANG"] = locale;
            }
            return View(vm2);
        }


        public ActionResult SearchCreateIndex()
        {
            LuceneSearch.AddUpdateLuceneIndex(LuceneSearchDataRepository.GetAll());
            TempData["Result"] = "Search index was created successfully!";
            return RedirectToAction("Index");
        }

        public ActionResult SearchOptimizeIndex()
        {
            LuceneSearch.Optimize();
            TempData["Result"] = "Search index was optimized successfully!";
            return RedirectToAction("Index");
        }





        public ActionResult Error(string locale, string code)
        {
            if (SSO_SessionTimeout())
            {
                SSO_ClearSession();
            }

            SSO_InternalKeepAlive();
            SSO_InternalHeartbeat();

            var session = getSession();
            if (session != null && !session.isKeptAlive)
            {
                Session["isKeptAlive"] = true;
            }


            if (Session["LANG"] != null)
            {
                locale = (string) Session["LANG"];
            }


            if (code == "404")
            {
                BaseViewModel vmp = BaseViewModel.make(locale, "page-not-found", null, Request, session);
                return View(vmp);
            }
            BaseViewModel vm2 = BaseViewModel.make(locale, "error" + code, null, Request, session);
            return View(vm2);
        }
    }
}