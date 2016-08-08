using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public override ActionResult Index()
        {
            return View();
        }
    }
}