using Frontend.Attributes;
using Frontend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class PageController : Controller
    {
        [Internationalization]
        public ActionResult Index(string locale, string category, string id)
        {
            BaseViewModel vm = BaseViewModel.make(locale, category, id);
            return View(vm);
        }
    }
}