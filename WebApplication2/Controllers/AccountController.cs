using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Security;
using WebApplication2.Context;
using WebApplication2.ViewModels;
using System.Data.Entity;
using System.Net;
using WebApplication2.Helpers;

namespace WebApplication2.Controllers
{
    public class AccountController : BaseController
    {
        MultiSelectList getRoleList(string selectedIDs = null)
        {
            List<string> ids = new List<string>();
            if (selectedIDs != null)
            {
                var selIDs = selectedIDs.Split(',');
                for (int i = 0; i < selIDs.Count(); i++)
                {
                    ids.Add(selIDs.ElementAt(i));
                }
            }

            var items = new List<string>();
            items.Add("superadmin");
            items.Add("editor");
            items.Add("approver");
            items.Add("publisher");
            return new MultiSelectList(items, ids.ToArray());
        }

        SelectList getAccountGroupsForSelect(int? selectedID = null)
        {
            var items = AccountGroupDbContext.getInstance().findGroups();
            return new SelectList(items, "AccountGroupID", "Name", selectedID);
        }

        SelectList getEmailNotificationsForSelect(int? selectedID = null)
        {
            // 0 = notify all items' changes in my user group
            // 1 = notify all items' changes created / approved / published by me
            // 2 = don't notify
            var items = new Dictionary<int, string>();
            items.Add(0, Account.getEmailNotificationRepresentation(0));
            items.Add(1, Account.getEmailNotificationRepresentation(1));
            items.Add(2, Account.getEmailNotificationRepresentation(2));
            return new SelectList(items.OrderBy(dict => dict.Value), "Key", "Value", selectedID);
        }





        // GET: Account
        [CustomAuthorize()]
        public override ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult List()
        {
            var items = AccountDbContext.getInstance().findAccounts();
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(items);
        }

        [CustomSuperadminOrEmpty()]
        public ActionResult Register()
        {
            ViewBag.GroupID = getAccountGroupsForSelect();
            ViewBag.RoleList = getRoleList();
            ViewBag.EmailNotifications = getEmailNotificationsForSelect();
            return View();
        }

        [CustomSuperadminOrEmpty()]
        [HttpPost]
        public ActionResult Register(Account account)
        {
            if (ModelState.IsValid)
            {
                var error = AccountDbContext.getInstance().tryRegisterAccount(account);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    ModelState.Clear();
                    TempData["Message"] = account.Firstname + " " + account.Lastname + " successfully registered.";
                    return RedirectToAction("List");
                }
            }
            ViewBag.GroupID = getAccountGroupsForSelect();
            ViewBag.RoleList = getRoleList();
            ViewBag.EmailNotifications = getEmailNotificationsForSelect();
            return View();
        }


        [CustomAuthorize()]
        public ActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }


        [HttpPost]
        public ActionResult Login(Account account)
        {
            var username = account.Username;
            var password = account.Password;

            var result = AccountDbContext.getInstance().tryLoginAccountByAccount(account);
            if (result != null)
            {
                if (result.LoginFails >= 3)
                {
                    return RedirectToAction("LoginLocked");
                }
                if (!result.isEnabled)
                {
                    return RedirectToAction("LoginDisabled");
                }
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Login failed");
            }
            return View();
        }



        public ActionResult LoginLocked()
        {
            return View();
        }


        public ActionResult LoginDisabled()
        {
            return View();
        }


        

        public ActionResult Logout()
        {
            AccountDbContext.getInstance().tryLogout();
            return RedirectToAction("Index");
        }


        [CustomAuthorize()]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize()]
        public ActionResult ChangePassword(AccountChangePasswordForm form)
        {
            var account = new Account();
            account.AccountID = SessionPersister.account.AccountID;
            account.Username = SessionPersister.account.Username;
            account.Password = form.OldPassword;

            var error = AccountDbContext.getInstance().tryChangePassword(account, form.Password, true);
            if (error == null)
            {
                return RedirectToAction("ChangePasswordSuccess");
            }
            else
            {
                ModelState.AddModelError("", error);
            }
            return View();
        }

        [CustomAuthorize()]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult Expired()
        {
            return View();
        }




        // DETAILS


        [CustomAuthorize()]
        public ActionResult Details(int id = 0)
        {
            var item = AccountDbContext.getInstance().findAccountByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(item);
        }


        [CustomAuthorize()]
        public ActionResult Me()
        {
            return Details(SessionPersister.account.AccountID);
        }






        // EDIT

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(int id = 0)
        {
            var item = AccountDbContext.getInstance().findAccountByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = getAccountGroupsForSelect(item.GroupID);
            ViewBag.RoleList = getRoleList(item.Role);
            ViewBag.EmailNotifications = getEmailNotificationsForSelect(item.EmailNotifications);
            return View(item);
        }



        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(Account item)
        {
            if (ModelState.IsValid)
            {
                var error = AccountDbContext.getInstance().tryEdit(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    ViewBag.Message = "Edit '" + item.Username + "' successfully";
                    ViewBag.GroupID = getAccountGroupsForSelect(item.GroupID);
                    ViewBag.RoleList = getRoleList(item.Role);
                    ViewBag.EmailNotifications = getEmailNotificationsForSelect(item.EmailNotifications);
                    return View(item);
                }
            }
            ViewBag.GroupID = getAccountGroupsForSelect(item.GroupID);
            ViewBag.RoleList = getRoleList(item.Role);
            ViewBag.EmailNotifications = getEmailNotificationsForSelect(item.EmailNotifications);
            return View(item);
        }





        // DELETE
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Delete(int id = 0)
        {
            var item = AccountDbContext.getInstance().findAccountByID(id);
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
            var item = AccountDbContext.getInstance().findAccountByID(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var name = item.Username;
            AccountDbContext.getInstance().tryDeleteAccount(item);
            TempData["Message"] = "Delete '" + name + "' successfully";
            return RedirectToAction("List");
        }







        // FORGOT PASSWORD

        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordForm form)
        {
            if (ModelState.IsValid)
            {
                var accounts = AccountDbContext.getInstance().findAccountsByEmail(form.email);
                foreach (Account acc in accounts)
                {
                    EmailHelper.SendEmailToSuperadminAccountsOnPasswordForget(acc);
                }
                return RedirectToAction("ForgotPasswordConfirm");
            }
            return View();
        }


        public ActionResult ForgotPasswordConfirm()
        {
            return View();
        }

    }
}