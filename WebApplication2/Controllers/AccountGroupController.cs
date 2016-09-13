using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Models;
using WebApplication2.Models.Infrastructure;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class AccountGroupController : Controller
    {
        MultiSelectList getAccessibleCategories(string selectedIDs = null)
        {
            List<int> ids = new List<int>();
            if (selectedIDs != null)
            {
                var selIDs = selectedIDs.Split(',');
                for (int i = 0; i < selIDs.Count(); i++)
                {
                    var id = selIDs.ElementAt(i);
                    if (!id.Equals(""))
                    {
                        ids.Add(Convert.ToInt32(selIDs.ElementAt(i)));
                    }
                }
            }

            var items = InfrastructureCategoryDbContext.getInstance().findAllCategorysAsNoTracking();
            return new MultiSelectList(items, "ItemID", "name_en", ids.ToArray());
        }

        


        // GET: InfrastructureCategory
        public ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize]
        public ActionResult List()
        {
            var items = AccountGroupDbContext.getInstance().findGroups();

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(items);
        }


        // CREATE

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Create()
        {
            ViewBag.AccessibleCategoryList = getAccessibleCategories();
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Create(AccountGroup item)
        {
            if (ModelState.IsValid)
            {
                item.isDefaultGroup = false;
                var error = AccountGroupDbContext.getInstance().create(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                    ViewBag.AccessibleCategoryList = getAccessibleCategories();
                    return View();
                }
                else
                {
                    ModelState.Clear();
                    TempData["Message"] = item.Name + " successfully created.";
                    return RedirectToAction("List");
                }
            }

            ViewBag.AccessibleCategoryList = getAccessibleCategories();
            return View();
        }





        // EDIT

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(int id = 0)
        {
            var item = AccountGroupDbContext.getInstance().findGroupByID(id);
            ViewBag.AccessibleCategoryList = getAccessibleCategories(item.AccessibleCategories);
            return View(item);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(AccountGroup item)
        {
            if (ModelState.IsValid)
            {
                var error = AccountGroupDbContext.getInstance().edit(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                    ViewBag.AccessibleCategoryList = getAccessibleCategories(item.AccessibleCategories);
                    return View(item);
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.AccessibleCategoryList = getAccessibleCategories(item.AccessibleCategories);
                    ViewBag.Message = "Edit '" + item.Name + "' successfully";
                    return View(item);
                }
            }
            else
            {
                ViewBag.AccessibleCategoryList = getAccessibleCategories(item.AccessibleCategories);
                return View(item);
            }
        }





        // DELETE

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Delete(int id = 0)
        {
            var item = AccountGroupDbContext.getInstance().findGroupByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult DeleteConfirmed(int id = 0)
        {
            var item = AccountGroupDbContext.getInstance().findGroupByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var name = item.Name;
            AccountGroupDbContext.getInstance().delete(item);
            TempData["Message"] = "Delete '" + item.Name + "' successfully";
            return RedirectToAction("List");
        }


    }
}