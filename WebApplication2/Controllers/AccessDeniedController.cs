using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class AccessDeniedController : BaseController
    {
        // GET: AccessDenied
        public override ActionResult Index()
        {
            return View();
        }
    }
}