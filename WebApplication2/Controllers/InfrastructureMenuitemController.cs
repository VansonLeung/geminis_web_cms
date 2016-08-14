using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Models.Infrastructure;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class InfrastructureMenuitemController : Controller
    {
        // GET: InfrastructureMenuitem
        public ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize]
        public ActionResult List(int? parentItemID = null)
        {
            var items = InfrastructureMenuitemDbContext.getInstance().findMenuItemsByParentID(parentItemID);
            return View(items);
        }



        // CREATE

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Create()
        {
            var parentItemsForSelect = InfrastructureMenuitemDbContext.getInstance().findMenuItemsByParentID();
            parentItemsForSelect.Insert(0, new Menuitem { ItemID = -1, name_en = "" });
            ViewBag.parentItemID = new SelectList(parentItemsForSelect, "ItemID", "name_en");
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Create(Menuitem item)
        {
            if (ModelState.IsValid)
            {
                InfrastructureMenuitemDbContext.getInstance().create(item);
                ModelState.Clear();
                ViewBag.Message = item.GetName() + " successfully created.";
            }
            if (item.parentItemID == null)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("List", new { parentItemID = item.parentItemID });
            }
        }





        // EDIT

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(int id = 0)
        {
            var item = InfrastructureMenuitemDbContext.getInstance().findMenuItemByID(id);
            
            var parentItemsForSelect = InfrastructureMenuitemDbContext.getInstance().findMenuItemsByParentIDAsNoTracking();
            parentItemsForSelect.Insert(0, new Menuitem { ItemID = -1, name_en = "" });
            ViewBag.parentItemID = new SelectList(parentItemsForSelect, "ItemID", "name_en", item.parentItemID);

            return View(item);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(Menuitem item)
        {
            if (ModelState.IsValid)
            {
                InfrastructureMenuitemDbContext.getInstance().edit(item);
                ModelState.Clear();

                var parentItemsForSelect = InfrastructureMenuitemDbContext.getInstance().findMenuItemsByParentIDAsNoTracking();
                parentItemsForSelect.Insert(0, new Menuitem { ItemID = -1, name_en = "" });
                ViewBag.parentItemID = new SelectList(parentItemsForSelect, "ItemID", "name_en", item.parentItemID);

                return View(item);
            }
            else
            {
                var parentItemsForSelect = InfrastructureMenuitemDbContext.getInstance().findMenuItemsByParentIDAsNoTracking();
                parentItemsForSelect.Insert(0, new Menuitem { ItemID = -1, name_en = "" });
                ViewBag.parentItemID = new SelectList(parentItemsForSelect, "ItemID", "name_en", item.parentItemID);

                return View(item);
            }
        }





        // DELETE

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Delete(int id = 0)
        {
            var item = InfrastructureMenuitemDbContext.getInstance().findMenuItemByID(id);
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
            var item = InfrastructureMenuitemDbContext.getInstance().findMenuItemByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var parentItemID = item.parentItemID;
            InfrastructureMenuitemDbContext.getInstance().delete(item, true);
            return RedirectToAction("List", new { parentItemID = parentItemID });
        }

        
    }
}