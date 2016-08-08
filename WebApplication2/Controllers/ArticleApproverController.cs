using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class ArticleApproverController : BaseController
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
            var items = db.findArticlesRequestingApproval();
            return View(items);
        }


        [CustomAuthorize()]
        public ActionResult Details(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }






        [CustomAuthorize(Roles = "superadmin,approver")]
        [HttpPost]
        public ActionResult ApproveArticle(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = db.tryRequestApproval(item);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(item);
            }
            else
            {
                return RedirectToAction("Details", new { id = item.ArticleID });
            }
        }

        [CustomAuthorize(Roles = "superadmin,approver")]
        [HttpPost]
        public ActionResult UnapproveArticle(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = db.tryRequestUnapproval(item);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(item);
            }
            else
            {
                return RedirectToAction("Details", new { id = item.ArticleID });
            }
        }

    }
}