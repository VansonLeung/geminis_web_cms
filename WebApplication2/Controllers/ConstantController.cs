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
    public class ConstantController : Controller
    {
        // GET: Constant
        [CustomAuthorize()]
        public ActionResult Index()
        {
            var items = ConstantDbContext.getInstance().find();
            return View(items);
        }

        // GET: Constant/Details/5
        [CustomAuthorize()]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Constant/Create
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Constant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Create(Constant item)
        {
            if (ModelState.IsValid)
            {
                var error = ConstantDbContext.getInstance().create(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                    return View();
                }
                else
                {
                    ModelState.Clear();
                    TempData["Message"] = item.Key + " successfully created.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        // GET: Constant/Edit/5
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(int id)
        {
            var item = ConstantDbContext.getInstance().findByID(id);
            return View(item);
        }

        // POST: Constant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(Constant item)
        {
            if (ModelState.IsValid)
            {
                var error = ConstantDbContext.getInstance().edit(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                    return View();
                }
                else
                {
                    ModelState.Clear();
                    TempData["Message"] = item.Key + " successfully edited.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        // GET: Constant/Delete/5
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Delete(int id)
        {
            var item = ConstantDbContext.getInstance().findByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Constant/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Delete(int id, Constant constant)
        {
            try
            {
                var item = ConstantDbContext.getInstance().findByID(id);
                if (item == null)
                {
                    return HttpNotFound();
                }
                var name = item.Key;
                ConstantDbContext.getInstance().delete(item);
                TempData["Message"] = "Delete '" + item.Key + "' successfully";
                return RedirectToAction("Index");
            }
            catch
            {
                return HttpNotFound();
            }
        }
    }
}
