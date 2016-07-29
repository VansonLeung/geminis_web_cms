using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class ArticleApproverController : Controller
    {
        ArticleDbContext db = new ArticleDbContext();

        // GET: ArticleApprover
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult List()
        {
            var items = db.findArticlesGroupByBaseVersion();
            return View(items);
        }

    }
}