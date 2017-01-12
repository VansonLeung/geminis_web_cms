using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Security;
using WebApplication2.ViewModels.Include;

namespace WebApplication2.Controllers
{
    public class PreviewController : Controller
    {
        [HttpPost, ValidateInput(false)]
        [CustomAuthorize()]
        public ActionResult Index(
            string name,
            string desc
            )
        {
            BaseViewModel vm = BaseViewModel.make(null, null, null, Request);

            vm.content = new ViewContent();
            vm.content.name = name;
            vm.content.desc = desc;
            vm.content.type = "ContentPage";

            return View(vm);
        }
    }
}