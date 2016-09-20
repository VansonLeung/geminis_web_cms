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
    public class ContentPageEditorController : BaseController
    {
        SelectList getCategoriesForSelect(int? selectedID = null)
        {
            var items = InfrastructureCategoryDbContext.getInstance().findAllCategorysContentPagesAsNoTracking();
            items.Insert(0, new Models.Infrastructure.Category { ItemID = -1, name_en = "" });
            return new SelectList(items, "ItemID", "name_en", selectedID);
        }

        // GET: ArticleEditor
        public override ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize]
        public ActionResult List()
        {
            var items = ContentPageDbContext.getInstance().findArticlesGroupByBaseVersion();
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
        public ActionResult Create(ContentPage contentPage)
        {
            if (ModelState.IsValid)
            {
                contentPage.BaseArticleID = 0;
                contentPage.Version = 0;
                ContentPageDbContext.getInstance().tryCreateNewArticle(contentPage);
                ModelState.Clear();
                TempData["Message"] = "'" + contentPage.Name + "' successfully created.";
                return RedirectToAction("DetailsLocale", new { baseArticleID = contentPage.BaseArticleID, version = contentPage.Version, lang = contentPage.Lang });
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
        public ActionResult CreateWithViewModelForm(ContentPageCreateForm form)
        {
            if (ModelState.IsValid)
            {
                // create empty article with base lang
                ContentPage article = form.makeBaseArticle();
                article.BaseArticleID = 0;
                article.Version = 0;
                ContentPageDbContext.getInstance().tryCreateNewArticle(article);

                var baseArticleID = article.BaseArticleID;
                var version = article.Version;

                // create locale articles

                var article_zh = form.makeLocaleArticle("zh");
                article_zh.BaseArticleID = baseArticleID;
                article_zh.Version = version;
                ContentPageDbContext.getInstance().tryCreateNewLocaleArticle(article_zh);

                var article_cn = form.makeLocaleArticle("cn");
                article_cn.BaseArticleID = baseArticleID;
                article_cn.Version = version;
                ContentPageDbContext.getInstance().tryCreateNewLocaleArticle(article_cn);

                ModelState.Clear();
                TempData["Message"] = article.Name + " successfully created.";
                return RedirectToAction("DetailsLocale", new { baseArticleID = article.BaseArticleID, version = article.Version, lang = article.Lang });
            }
            else
            {
                ViewBag.categoryID = getCategoriesForSelect();
                return View();
            }
        }







        // EDIT LOCALE
        [CustomAuthorize(Roles = "superadmin,editor")]
        public ActionResult UpsertLocale(int baseArticleID = 0, int version = 0, String lang = null)
        {
            // find existing locale article for base article ID and version and lang
            ContentPage item = ContentPageDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ContentPageDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
                if (baseArticle == null)
                {
                    return HttpNotFound();
                }

                var article = new ContentPage();
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
        public ActionResult UpsertLocale(ContentPage item)
        {
            if (ModelState.IsValid)
            {
                String error = null;
                if (item.ArticleID == 0)
                {
                    // create
                    error = ContentPageDbContext.getInstance().tryCreateNewLocaleArticle(item);
                }
                else
                {
                    // edit
                    error = ContentPageDbContext.getInstance().tryEditArticle(item);
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
            ContentPage item = ContentPageDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ContentPageDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
                if (baseArticle == null)
                {
                    return HttpNotFound();
                }

                var article = new ContentPage();
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
        public ActionResult EditProperties(ContentPage item)
        {
            if (ModelState.IsValid)
            {
                var error = ContentPageDbContext.getInstance().tryEditArticleProperties(item, true);
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
            ContentPage item = ContentPageDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ContentPageDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
                if (baseArticle == null)
                {
                    return HttpNotFound();
                }

                var article = new ContentPage();
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
            ContentPage item = ContentPageDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, lang);
            if (item == null)
            {
                // if locale not exists, create blank form, while inheriting latest base article's version
                var baseArticle = ContentPageDbContext.getInstance().findLatestArticleByBaseArticleID(baseArticleID, null);
                if (baseArticle == null)
                {
                    return HttpNotFound();
                }

                var article = new ContentPage();
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
            var item = ContentPageDbContext.getInstance().findArticleByID(id);
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
            var item = ContentPageDbContext.getInstance().findArticleByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var name = item.Name;
            var error = ContentPageDbContext.getInstance().tryDeleteArticle(item, true);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(item);
            }
            TempData["Message"] = "'" + name + "' Deleted";
            return RedirectToAction("List");
        }



        
    }
}