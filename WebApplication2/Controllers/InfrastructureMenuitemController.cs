using System;
using System.Collections.Generic;
using System.IO;
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
        SelectList getParentItemsForSelect(int? selectedID = null)
        {
            var parentItemsForSelect = InfrastructureMenuitemDbContext.getInstance().findMenuItemsByParentIDAsNoTracking();
            parentItemsForSelect.Insert(0, new Menuitem { ItemID = -1, name_en = "" });
            return new SelectList(parentItemsForSelect, "ItemID", "name_en", selectedID);
        }

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
            ViewBag.parentItemID = getParentItemsForSelect();
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Create(Menuitem item, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string ImageName = Path.GetFileName(image.FileName);
                    string physicalPath = Server.MapPath("~/images/uploads/" + ImageName);
                    image.SaveAs(physicalPath);
                    item.imagePath = physicalPath;
                }

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
            ViewBag.parentItemID = getParentItemsForSelect(item.parentItemID);
            return View(item);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(Menuitem item, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string ImageName = Path.GetFileName(image.FileName);
                    string physicalPath = Server.MapPath("~/images/uploads/" + ImageName);
                    image.SaveAs(physicalPath);
                    item.imagePath = "/images/uploads/" + ImageName;
                }

                var error = InfrastructureMenuitemDbContext.getInstance().edit(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                    ViewBag.parentItemID = getParentItemsForSelect(item.parentItemID);
                    return View(item);
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.parentItemID = getParentItemsForSelect(item.parentItemID);
                    return View(item);
                }
            }
            else
            {
                ViewBag.parentItemID = getParentItemsForSelect(item.parentItemID);
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