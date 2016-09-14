using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Models.Infrastructure;
using WebApplication2.Properties;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class InfrastructureCategoryController : Controller
    {
        SelectList getParentItemsForSelect(int? selectedID = null)
        {
            var parentItemsForSelect = InfrastructureCategoryDbContext.getInstance().findCategorysByParentID();
            parentItemsForSelect.Insert(0, new Category { ItemID = -1, name_en = "" });
            return new SelectList(parentItemsForSelect, "ItemID", "name_en", selectedID);
        }

        // GET: InfrastructureCategory
        public ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize]
        public ActionResult List(int? parentItemID = null)
        {
            if (parentItemID != null)
            {
                var rootItem = InfrastructureCategoryDbContext.getInstance().findCategoryByID(parentItemID);
                if (rootItem != null)
                {
                    ViewBag.subcategory = rootItem;
                }
            }

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            var items = InfrastructureCategoryDbContext.getInstance().findCategorysByParentID(parentItemID);
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
        public ActionResult Create(Category item, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string ImageName = Path.GetFileName(image.FileName);
                    string physicalPath = Server.MapPath("~" + Settings.Default.MS_IMAGE_UPLOAD_SRC + ImageName);
                    image.SaveAs(physicalPath);
                    item.imagePath = physicalPath;
                }

                InfrastructureCategoryDbContext.getInstance().create(item);
                ModelState.Clear();
                ViewBag.Message = item.GetName() + " successfully created.";
            }
            else
            {
                ViewBag.parentItemID = getParentItemsForSelect();
                return View();
            }
            TempData["Message"] = "'" + item.GetName() + "' successfully created.";
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
            var item = InfrastructureCategoryDbContext.getInstance().findCategoryByID(id);
            ViewBag.parentItemID = getParentItemsForSelect(item.parentItemID);
            return View(item);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(Category item, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string ImageName = Path.GetFileName(image.FileName);
                    string physicalPath = Server.MapPath("~" + Settings.Default.MS_IMAGE_UPLOAD_SRC + ImageName);
                    image.SaveAs(physicalPath);
                    item.imagePath = physicalPath;
                }

                ViewBag.Message = "Edit '" + item.GetName() + "' successfully";
                InfrastructureCategoryDbContext.getInstance().edit(item);
                ModelState.Clear();
                ViewBag.parentItemID = getParentItemsForSelect(item.parentItemID);
                return View(item);
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
            var item = InfrastructureCategoryDbContext.getInstance().findCategoryByID(id);
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
            var item = InfrastructureCategoryDbContext.getInstance().findCategoryByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var name = item.GetName();
            var parentItemID = item.parentItemID;
            var error = InfrastructureCategoryDbContext.getInstance().delete(item, true);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["Message"] = "'" + name + "' Deleted";
            }
            return RedirectToAction("List", new { parentItemID = parentItemID });
        }


    }
}