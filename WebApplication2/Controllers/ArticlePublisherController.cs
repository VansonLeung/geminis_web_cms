using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class ArticlePublisherController : BaseController
    {
        ArticlePublishedDbContext dbPublished = new ArticlePublishedDbContext();
        ArticleDbContext db = new ArticleDbContext();

        // GET: ArticlePublisher
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult ListArticlesPublished()
        {
            var items = dbPublished.findPublishedArticlesGroupByBaseVersion();
            return View(items);
        }


        [CustomAuthorize]
        public ActionResult ListArticlesAvailable()
        {
            var items = db.findArticlesGroupByBaseVersion();
            return View(items);
        }





        [CustomAuthorize(Roles = "superadmin,publisher")]
        [HttpPost]
        public ActionResult PublishArticle(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = dbPublished.tryPublishArticle(item, db);
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

        [CustomAuthorize(Roles = "superadmin,publisher")]
        [HttpPost]
        public ActionResult UnpublishArticle(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = dbPublished.tryUnpublishArticle(item, db);
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