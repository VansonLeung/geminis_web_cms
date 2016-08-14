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
        // GET: ArticleEditor
        public override ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize]
        public ActionResult List()
        {
            var items = ArticleDbContext.getInstance().findArticlesGroupByBaseVersion();
            return View(items);
        }


        [CustomAuthorize]
        public ActionResult ListArticleVersions(int baseArticleID = 0)
        {
            var article = new Article();
            article.BaseArticleID = baseArticleID;
            var items = ArticleDbContext.getInstance().findAllArticlesByBaseArticle(article);
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
                article.Version = 0;
                ArticleDbContext.getInstance().tryCreateNewArticle(article);
                ModelState.Clear();
                ViewBag.Message = article.Name + " successfully created.";
            }
            return RedirectToAction("DetailsLocale", new { baseArticleID = article.BaseArticleID, version = article.Version, lang = article.Lang });
        }






        // CREATE NEW VERSION

        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult CreateNewVersion(int baseArticleID = 0)
        {
            var article = ArticleDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID);
            return View(article);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult CreateNewVersion(Article article)
        {
            if (ModelState.IsValid)
            {
                article.Version = 0;
                ArticleDbContext.getInstance().tryCreateNewArticle(article);
                ModelState.Clear();
                ViewBag.Message = "New Version of " + article.Name + " with version: " + article.Version + " successfully created.";
            }
            return RedirectToAction("UpsertLocale", new { baseArticleID = article.BaseArticleID, version = article.Version });
        }





        


        // EDIT LOCALE
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult UpsertLocale(int baseArticleID = 0, int version = 0, String lang = null)
        {
            // find existing locale article for base article ID and version and lang
            Article item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ArticleDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
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
                    error = ArticleDbContext.getInstance().tryCreateNewLocaleArticle(item);
                }
                else
                {
                    // edit
                    error = ArticleDbContext.getInstance().tryEditArticle(item);
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



        // EDIT PROPERTIES

        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult EditProperties(int baseArticleID = 0, int version = 0, String lang = null)
        {
            Article item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ArticleDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
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
        public ActionResult EditProperties(Article item)
        {
            if (ModelState.IsValid)
            {
                var error = ArticleDbContext.getInstance().tryEditArticleProperties(item, true);
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







        



        // DETAILS LOCALE
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult DetailsLocale(int baseArticleID = 0, int version = 0, String lang = null)
        {
            // find existing locale article for base article ID and version and lang
            Article item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ArticleDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
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


        // DETAILS PROPERTIES
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult DetailsProperties(int baseArticleID = 0, int version = 0, String lang = null)
        {
            // find existing locale article for base article ID and version and lang
            Article item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ArticleDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
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












        // DELETE
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult Delete(int id = 0)
        {
            var item = ArticleDbContext.getInstance().findArticleByID(id);
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
            var item = ArticleDbContext.getInstance().findArticleByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = ArticleDbContext.getInstance().tryDeleteArticle(item);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(id);
            }
            return RedirectToAction("List");
        }







        // SUBMIT REQUEST FOR APPROVAL
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult SubmitRequestForApproval(int baseArticleID = 0, int version = 0, String lang = null)
        {
            // find existing locale article for base article ID and version and lang
            Article item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                return HttpNotFound();
            }
            else
            {
                // if locale exists, treat as edit form
            }
            return View(item);
        }


        [CustomAuthorize(Roles = "superadmin,editor")]
        [HttpPost]
        public ActionResult SubmitRequestForApproval(Article item)
        {
            if (ModelState.IsValid)
            {
                var error = ArticleDbContext.getInstance().trySubmitRequestForApproval(item, true);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    return RedirectToAction("UpsertLocale", new { baseArticleID = item.BaseArticleID, version = item.Version });
                }
            }
            return View(item);
        }
    }
}