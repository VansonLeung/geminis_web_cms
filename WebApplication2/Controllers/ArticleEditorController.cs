using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Models;
using WebApplication2.Security;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    public class ArticleEditorController : BaseController
    {
        SelectList getCategoriesForSelect(int? selectedID = null)
        {
            var items = InfrastructureCategoryDbContext.getInstance().findCategorysInTreeExcept();
            items.Insert(0, new Models.Infrastructure.Category { ItemID = -1, url = "" });
            foreach (var cat in items)
            {
                for (int i = 0; i < cat.itemLevel; i++)
                {
                    cat.url = " > " + cat.url;
                }
            }
            return new SelectList(items, "ItemID", "url", selectedID);
        }

        // GET: ArticleEditor
        public override ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult List()
        {
            var items = ArticleDbContext.getInstance().findArticlesGroupByBaseVersion();
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(items);
        }


        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult ListArticleVersions(int baseArticleID = 0)
        {
            var article = new Article();
            article.BaseArticleID = baseArticleID;
            var items = ArticleDbContext.getInstance().findAllArticlesByBaseArticle(article);
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(items);
        }





        // CREATE

        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult Create()
        {
            ViewBag.categoryID = getCategoriesForSelect();
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
                return RedirectToAction("DetailsLocale", new { baseArticleID = article.BaseArticleID, version = article.Version, lang = article.Lang });
            }
            else
            {
                ViewBag.categoryID = getCategoriesForSelect();
                return View();
            }
        }




        // CREATE WITH CUSTOM VIEWMODEL FORM

        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult CreateWithViewModelForm()
        {
            ViewBag.categoryID = getCategoriesForSelect();
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult CreateWithViewModelForm(ArticleCreateForm form)
        {
            if (ModelState.IsValid)
            {
                // create empty article with base lang
                Article article = form.makeBaseArticle();
                article.BaseArticleID = 0;
                article.Version = 0;
                ArticleDbContext.getInstance().tryCreateNewArticle(article);

                var baseArticleID = article.BaseArticleID;
                var version = article.Version;

                // create locale articles

                var article_zh = form.makeLocaleArticle("zh");
                article_zh.BaseArticleID = baseArticleID;
                article_zh.Version = version;
                ArticleDbContext.getInstance().tryCreateNewLocaleArticle(article_zh);

                var article_cn = form.makeLocaleArticle("cn");
                article_cn.BaseArticleID = baseArticleID;
                article_cn.Version = version;
                ArticleDbContext.getInstance().tryCreateNewLocaleArticle(article_cn);

                ModelState.Clear();
                TempData["Message"] = "'" + article.Name + "' successfully created.";
                return RedirectToAction("DetailsLocale", new { baseArticleID = article.BaseArticleID, version = article.Version, lang = article.Lang });
            }
            else
            {
                ViewBag.categoryID = getCategoriesForSelect();
                return View();
            }
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
                article.Lang = null;
                article.Version = 0;
                var error = ArticleDbContext.getInstance().tryCreateNewArticle(article);
                if (error == null)
                {
                    ModelState.Clear();
                    var a = ArticleDbContext.getInstance().findLatestArticleByBaseArticle(article);
                    TempData["Message"] = "New Version of '" + a.Name + "' with version: " + a.Version + " successfully created.";
                    return RedirectToAction("DetailsLocale", new { baseArticleID = a.BaseArticleID, version = a.Version, lang = a.Lang });
                }
                else
                {
                    ModelState.AddModelError("", error);
                    return View();
                }
            }
            else
            {
                return View();
            }
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

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
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
                    ViewBag.Message = "Edit '" + item.Name + "' successfully";
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
            ViewBag.categoryID = getCategoriesForSelect(item.categoryID);
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
                    ViewBag.categoryID = getCategoriesForSelect(item.categoryID);
                    ViewBag.Message = "Edit '" + item.Name + "' successfully";
                    return View(item);
                }
            }
            ViewBag.categoryID = getCategoriesForSelect(item.categoryID);
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

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
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
            var name = item.Name;
            var error = ArticleDbContext.getInstance().tryDeleteArticle(item, true);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(item);
            }
            TempData["Message"] = "'" + name + "' Deleted";
            return RedirectToAction("List");
        }





        // DELETE
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult DeleteSingle(int id = 0)
        {
            var item = ArticleDbContext.getInstance().findArticleByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }


        [HttpPost, ActionName("DeleteSingle")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult DeleteSingleConfirmed(int id = 0)
        {
            var item = ArticleDbContext.getInstance().findArticleByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var name = item.Name;
            var baseArticleID = item.BaseArticleID;
            var error = ArticleDbContext.getInstance().tryDeleteArticle(item, false);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(item);
            }
            TempData["Message"] = "'" + name + "' Deleted";
            return RedirectToAction("ListArticleVersions", new { baseArticleID = baseArticleID });
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
                    TempData["Message"] = "'" + item.Name + "' Submitted for Approval";
                    return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = item.Lang });
                }
            }
            return View(item);
        }
    }
}