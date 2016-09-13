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
    public class SystemMaintenanceNotificationController : Controller
    {
        // GET: SystemMaintenance


        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Index()
        {
            var items = SystemMaintenanceNotificationDbContext.getInstance().findAllNotifications();

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(items);
        }

        

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult CreateScheduled()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult CreateScheduled(SystemMaintenanceNotification item)
        {
            if (ModelState.IsValid)
            {
                var error = SystemMaintenanceNotificationDbContext.getInstance().createScheduledNotification(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                    return View();
                }
                else
                {
                    ModelState.Clear();
                    TempData["Message"] = "New notification successfully scheduled.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View();
            }
        }







        // EDIT
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(int id = 0)
        {
            var item = SystemMaintenanceNotificationDbContext.getInstance().findNotificationByID(id);
            return View(item);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(SystemMaintenanceNotification item)
        {
            if (ModelState.IsValid)
            {
                var error = SystemMaintenanceNotificationDbContext.getInstance().editNotification(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                    return View(item);
                }
                else
                {
                    ModelState.Clear();
                    return View(item);
                }
            }
            else
            {
                return View(item);
            }
        }





        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Activate(int id = 0)
        {
            var item = SystemMaintenanceNotificationDbContext.getInstance().findNotificationByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var error = SystemMaintenanceNotificationDbContext.getInstance().activateNotification(item);
            if (error != null)
            {
                ViewBag.Message = error;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Successfully activated.";
                return RedirectToAction("Index");
            }
        }




        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Deactivate(int id = 0)
        {
            var item = SystemMaintenanceNotificationDbContext.getInstance().findNotificationByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var error = SystemMaintenanceNotificationDbContext.getInstance().deactivateNotification(item);
            if (error != null)
            {
                ViewBag.Message = error;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Successfully deactivated.";
                return RedirectToAction("Index");
            }
        }

    }
}