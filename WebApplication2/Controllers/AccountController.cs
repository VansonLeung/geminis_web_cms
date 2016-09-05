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
            items.Insert(0, new AccountGroup { AccountGroupID = -1, Name = "" });
            return new SelectList(items, "AccountGroupID", "Name", selectedID);
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
            return View(items);
        }

        [CustomSuperadminOrEmpty()]
        public ActionResult Register()
        {
            ViewBag.GroupID = getAccountGroupsForSelect();
            ViewBag.RoleList = getRoleList();
            return View();
        }

        [CustomSuperadminOrEmpty()]
        [HttpPost]
        public ActionResult Register(Account account)
        {
            if (ModelState.IsValid)
            {
                AccountDbContext.getInstance().tryRegisterAccount(account);
                ModelState.Clear();
                ViewBag.Message = account.Firstname + " " + account.Lastname + " successfully registered.";
                return RedirectToAction("List");
            }
            ViewBag.GroupID = getAccountGroupsForSelect();
            ViewBag.RoleList = getRoleList();
            return View();
        }


        public ActionResult Login()
        {
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
                ModelState.AddModelError("", "Wrong username / password combination");
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
                    return RedirectToAction("Details", new { id = item.AccountID });
                }
            }
            ViewBag.GroupID = getAccountGroupsForSelect(item.GroupID);
            ViewBag.RoleList = getRoleList(item.Role);
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
            AccountDbContext.getInstance().tryDeleteAccount(item);
            return RedirectToAction("List");
        }







        // FORGOT PASSWORD

        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var accounts = AccountDbContext.getInstance().findAccountsByEmail(email);
            foreach (Account acc in accounts)
            {
                EmailHelper.SendEmailToSuperadminAccountsOnPasswordForget(acc);
            }
            return RedirectToAction("ForgotPasswordConfirm");
        }


        public ActionResult ForgotPasswordConfirm()
        {
            return View();
        }

    }
}