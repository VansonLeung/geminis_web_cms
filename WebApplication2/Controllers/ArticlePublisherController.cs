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
    public class ArticlePublisherController : BaseController
    {
        // GET: ArticlePublisher
        public override ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Roles = "superadmin,publisher")]
        public ActionResult ListArticlesPublished()
        {
            var items = ArticlePublishedDbContext.getInstance().findPublishedArticlesGroupByBaseVersion();
            return View(items);
        }


        [CustomAuthorize(Roles = "superadmin,publisher")]
        public ActionResult ListArticlesApproved()
        {
            var items = ArticleDbContext.getInstance().findArticlesGroupByBaseVersionApproved();
            return View(items);
        }


        [CustomAuthorize(Roles = "superadmin,publisher")]
        public ActionResult ListArticleVersions(int baseArticleID = 0)
        {
            var article = new Article();
            article.BaseArticleID = baseArticleID;
            var items = ArticleDbContext.getInstance().findAllArticlesByBaseArticle(article);
            return View(items);
        }



        // DETAILS LOCALE
        [CustomAuthorize(Roles = "superadmin,publisher")]
        public ActionResult DetailsLocale(int baseArticleID = 0, int version = 0, String lang = null)
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
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(item);
        }


        // DETAILS LOCALE
        [CustomAuthorize(Roles = "superadmin,publisher")]
        public ActionResult DetailsProperties(int baseArticleID = 0, int version = 0, String lang = null)
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







        [HttpPost]
        [CustomAuthorize(Roles = "superadmin,publisher")]
        public ActionResult PublishArticle(Article article)
        {
            if (ModelState.IsValid)
            {
                var datePublishStart = article.datePublishStart;
                var datePublishEnd = article.datePublishEnd;

                var item = ArticleDbContext.getInstance().findArticleByVersionAndLang(article.BaseArticleID, article.Version, "en");
                if (item == null)
                {
                    return HttpNotFound();
                }

                if (datePublishStart != null
                    && datePublishEnd != null)
                {
                    var start = datePublishStart.GetValueOrDefault();
                    var end = datePublishEnd.GetValueOrDefault();

                    if (start.CompareTo(end) > 0)
                    {
                        TempData["ErrorMessage"] = "Publish end date must be greater than publish start date";
                        return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
                    }
                }

                item.datePublishStart = datePublishStart;
                item.datePublishEnd = datePublishEnd;
                var error = ArticlePublishedDbContext.getInstance().tryPublishArticle(item, true);
                if (error != null)
                {
                    TempData["ErrorMessage"] = error;
                    return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
                }
                else
                {
                    TempData["Message"] = "'" + item.Name + "' Published";
                    return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
                }
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        TempData["ErrorMessage"] = error.ErrorMessage;
                    }
                }
                var item = ArticleDbContext.getInstance().findArticleByVersionAndLang(article.BaseArticleID, article.Version, "en");
                return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
            }
        }

        [CustomAuthorize(Roles = "superadmin,publisher")]
        public ActionResult UnpublishArticle(int baseArticleID = 0, int version = 0)
        {
            var item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, "en");
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = ArticlePublishedDbContext.getInstance().tryUnpublishArticle(item, true);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
            }
            else
            {
                TempData["Message"] = "'" + item.Name + "' Unpublished";
                return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
            }
        }

    }
}