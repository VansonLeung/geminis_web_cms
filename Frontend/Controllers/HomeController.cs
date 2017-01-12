using Frontend.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.ViewModels.Include;

namespace Frontend.Controllers
{
    public class HomeController : BaseController
    {
        [Internationalization]
        public override ActionResult Index()
        {
            BaseViewModel vm = BaseViewModel.make(null, null, null, Request);
            return View(vm);
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