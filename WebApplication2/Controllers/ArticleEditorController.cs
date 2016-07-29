using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class ArticleEditorController : BaseController
    {
        ArticleDbContext db = new ArticleDbContext();

        // GET: ArticleEditor
        public override ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize]
        public ActionResult List()
        {
            var items = db.findArticlesGroupByBaseVersion();
            return View(items);
        }




        // CREATE

        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                article.BaseArticleID = 0;
                db.tryCreateNewArticle(article);
                ModelState.Clear();
                ViewBag.Message = article.Name + " successfully created.";
            }
            return RedirectToAction("Details", new { id = article.ArticleID });
        }








        // CREATE NEW VERSION

        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult CreateNewVersion(int baseArticleID = 0)
        {
            var article = db.findLatestArticleByBaseArticleID(baseArticleID);
            return View(article);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult CreateNewVersion(Article article)
        {
            if (ModelState.IsValid)
            {
                db.tryCreateNewArticle(article);
                ModelState.Clear();
                ViewBag.Message = "New Version of " + article.Name + " with version: " + article.Version + " successfully created.";
            }
            return RedirectToAction("Details", new { id = article.ArticleID });
        }









        // EDIT

        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult Edit(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }



        [HttpPost]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult Edit(Article item)
        {
            if (ModelState.IsValid)
            {
                var error = db.tryEditArticle(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    return RedirectToAction("Details", new { id = item.ArticleID });
                }
            }
            return View(item);
        }




        // EDIT LOCALE
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult UpsertLocale(int baseArticleID = 0, int version = 0, String lang = null)
        {
            // find existing locale article for base article ID and version and lang
            Article item = db.findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = db.findLatestArticleByBaseArticleID(baseArticleID, null);
                if (baseArticle == null)
                {
                    return HttpNotFound();
                }

                var article = new Article();
                article.BaseArticleID = baseArticle.BaseArticleID;
                article.Lang = lang;
                article.Version = baseArticle.Version;
                item = article;
            }
            else
            {
                // if locale exists, treat as edit form
            }
            return View(item);
        }
        


        [HttpPost]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult UpsertLocale(Article item)
        {
            if (ModelState.IsValid)
            {
                String error = null;
                if (item.ArticleID == 0)
                {
                    // create
                    error = db.tryCreateNewLocaleArticle(item);
                }
                else
                {
                    // edit
                    error = db.tryEditArticle(item);
                }

                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    return View(item);
                }
            }
            return View(item);
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













        // DELETE
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult Delete(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult DeleteConfirmed(int id = 0)
        {
            var item = db.articleDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            db.articleDb.Remove(item);
            db.SaveChanges();
            return RedirectToAction("List");
        }
    }
}