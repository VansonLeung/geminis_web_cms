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
    public class ArticleApproverController : BaseController
    {
        // GET: ArticleApprover
        public override ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Roles = "superadmin,approver")]
        public ActionResult List()
        {
            var items = ArticleDbContext.getInstance().findArticlesRequestingApproval();
            return View(items);
        }


        // DETAILS LOCALE
        [CustomAuthorize(Roles = "superadmin,approver")]
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
            return View(item);
        }


        // DETAILS LOCALE
        [CustomAuthorize(Roles = "superadmin,approver")]
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



        /*

        [CustomAuthorize(Roles = "superadmin,approver")]
        public ActionResult ApproveArticle(int baseArticleID = 0, int version = 0)
        {
            var item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, "en");
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = ArticleDbContext.getInstance().tryRequestApproval(item, true);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
            }
            else
            {
                return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
            }
        }

        [CustomAuthorize(Roles = "superadmin,approver")]
        public ActionResult UnapproveArticle(int baseArticleID = 0, int version = 0)
        {
            var item = ArticleDbContext.getInstance().findArticleByVersionAndLang(baseArticleID, version, "en");
            if (item == null)
            {
                return HttpNotFound();
            }
            var error = ArticleDbContext.getInstance().tryRequestUnapproval(item, true);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
            }
            else
            {
                return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = "en" });
            }
        }

    */


        // SUBMIT REQUEST FOR APPROVAL
        [CustomAuthorize(Roles = "superadmin,approver")]
        public ActionResult ApproveArticle(int baseArticleID = 0, int version = 0, String lang = null)
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


        [CustomAuthorize(Roles = "superadmin,approver")]
        [HttpPost]
        public ActionResult ApproveArticle(Article item)
        {
            if (ModelState.IsValid)
            {
                var error = ArticleDbContext.getInstance().tryRequestApproval(item, true);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = item.Lang });
                }
            }
            return View(item);
        }



        // SUBMIT REQUEST FOR APPROVAL
        [CustomAuthorize(Roles = "superadmin,approver")]
        public ActionResult UnapproveArticle(int baseArticleID = 0, int version = 0, String lang = null)
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


        [CustomAuthorize(Roles = "superadmin,approver")]
        [HttpPost]
        public ActionResult UnapproveArticle(Article item)
        {
            if (ModelState.IsValid)
            {
                var error = ArticleDbContext.getInstance().tryRequestUnapproval(item, true);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    return RedirectToAction("DetailsLocale", new { baseArticleID = item.BaseArticleID, version = item.Version, lang = item.Lang });
                }
            }
            return View(item);
        }
    }
}