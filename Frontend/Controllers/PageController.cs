using Frontend.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.ViewModels.Include;
using static Frontend.Controllers.SessionController;

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

                return RedirectToRoute(new
                {
                    controller = "Page",
                    action = "Index",
                    locale = locale,
                    category = "session_timeout",
                });
            }

            if (category == "session_timeout")
            {
                category = "login";
                id = null;
                BaseViewModel vml = BaseViewModel.make(locale, category, id, Request, getSession(true));

                var min = new SessionLogin().getSessionKeepaliveMinutes();

                if (locale == "zh-HK" || locale == "zh-TW" || locale == "zh")
                {
                    ViewBag.message = "登入時間以空置了超過" + min + "分鐘，請重新登入";
                }
                else if (locale == "zh-CN" || locale == "cn")
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
                return View(vml);
            }

            SSO_InternalKeepAlive();
            SSO_InternalHeartbeat();

            var session = getSession();
            if (session != null && !session.isKeptAlive)
            {
                Session["isKeptAlive"] = true;

                /*
                if (session.hasTradingAcc)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Page",
                        action = "Index",
                        locale = locale,
                        category = "trading",
                    });
                }
                */
            }

            if (category != null && category.ToLower() == "home")
            {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index",
                    locale = locale
                });
            }

            BaseViewModel vm = BaseViewModel.make(locale, category, id, Request, session);

            if (!vm.category.isVisitor)
            {
                if (vm.current.session == null
                    || vm.current.session.clientID == null)
                {
                    // HOME
                    var redirect_path = Request.Path;
                    if (redirect_path.ToLower().Contains("/login")
                        || redirect_path.ToLower().Contains("/session_timeout"))
                    {
                        redirect_path = "/" + locale;
                    }

                    category = "login";
                    id = null;
                    BaseViewModel vml = BaseViewModel.make(locale, category, id, Request, getSession(true));
                    vml.redirectPack = vm;
                    vml.constants.Add(new WebApplication2.Models.Constant
                    {
                        Key = "redirect",
                        Value = redirect_path,
                        isActive = true,
                    });

                    if (locale == "zh-HK" || locale == "zh-TW" || locale == "zh")
                    {
                        ViewBag.message = "請登入";
                    }
                    else if (locale == "zh-CN" || locale == "cn")
                    {
                        ViewBag.message = "请登入";
                    }
                    else
                    {
                        ViewBag.message = "Please login";
                    }

                    if (locale != null)
                    {
                        Session["LANG"] = locale;
                    }
                    return View(vml);
                }
            }

            if (!vm.category.isVisitor
                && !vm.category.isMember)
            {
                if (vm.current.session == null
                    || vm.current.session.clientID == null
                    || vm.current.session.hasTradingAcc == false)
                {
                    // TRADING PAGE
                    var redirect_path = Request.Path;
                    if (redirect_path.ToLower().Contains("/login")
                        || redirect_path.ToLower().Contains("/session_timeout"))
                    {
                        redirect_path = "/" + locale + "/page/trading";
                    }

                    category = "login";
                    id = null;
                    BaseViewModel vml = BaseViewModel.make(locale, category, id, Request, getSession(true));
                    vml.redirectPack = vm;
                    vml.constants.Add(new WebApplication2.Models.Constant
                    {
                        Key = "redirect",
                        Value = redirect_path,
                        isActive = true,
                    });

                    if (locale == "zh-HK" || locale == "zh-TW" || locale == "zh")
                    {
                        ViewBag.message = "請登入交易账号";
                    }
                    else if (locale == "zh-CN" || locale == "cn")
                    {
                        ViewBag.message = "请登入交易账号";
                    }
                    else
                    {
                        ViewBag.message = "Please login as trading account";
                    }

                    if (locale != null)
                    {
                        Session["LANG"] = locale;
                    }
                    return View(vml);
                }
            }

            if (locale != null)
            {
                Session["LANG"] = locale;
            }
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