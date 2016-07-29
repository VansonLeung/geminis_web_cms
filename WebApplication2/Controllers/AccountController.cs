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

namespace WebApplication2.Controllers
{
    public class AccountController : BaseController
    {
        AccountDbContext db = new AccountDbContext();

        // GET: Account
        public override ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult List()
        {
            var items = db.findAccounts();
            return View(items);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Account account)
        {
            if (ModelState.IsValid)
            {
                db.tryRegisterAccount(account);
                ModelState.Clear();
                ViewBag.Message = account.Firstname + " " + account.Lastname + " successfully registered.";
            }
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

            var result = db.tryLoginAccountByAccount(account);
            if (result != null)
            {
                if (result.LoginFails >= 3)
                {
                    return RedirectToAction("LoginLocked");
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



        public ActionResult ForgotPassword()
        {
            return View();
        }



        public ActionResult Logout()
        {
            db.tryLogout();
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
            account.Username = SessionPersister.account.Username;
            account.Password = form.OldPassword;

            var error = db.tryChangePassword(account, form.Password);
            if (error == null)
            {
                db.Entry(account).State = EntityState.Modified;
                account.NeedChangePassword = false;
                db.SaveChanges();
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

        public ActionResult ChangePasswordRequest()
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
            var item = db.accountDb.Find(id);
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
            var item = db.accountDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }



        [HttpPost]
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Edit(Account item)
        {
            if (ModelState.IsValid)
            {
                var error = db.tryEdit(item);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    return RedirectToAction("Details", new { id = item.AccountID });
                }
            }
            return View(item);
        }





        // DELETE
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Delete(int id = 0)
        {
            var item = db.accountDb.Find(id);
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
            var item = db.accountDb.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            db.accountDb.Remove(item);
            db.SaveChanges();
            return RedirectToAction("List");
        }
    }
}