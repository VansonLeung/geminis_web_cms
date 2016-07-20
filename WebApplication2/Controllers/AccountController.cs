using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Security;
using WebApplication2.Context;

namespace WebApplication2.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public override ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult List()
        {
            using (AccountDbContext db = new AccountDbContext())
            {
                var items = db.findAccounts();
                return View(items);
            }
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
                using (AccountDbContext db = new AccountDbContext())
                {
                    db.tryRegisterAccount(account);
                }
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
            using (AccountDbContext db = new AccountDbContext())
            {
                var username = account.Username;
                var password = account.Password;

                var result = db.tryLoginAccountByAccount(account);
                if (result != null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong username / password combination");
                }
            }
            return View();
        }



        public ActionResult Logout()
        {
            using (AccountDbContext db = new AccountDbContext())
            {
                db.tryLogout();
            }
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

            using (AccountDbContext db = new AccountDbContext())
            {
                var error = db.tryChangePassword(account, form.Password);
                if (error == null)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                } 
                else
                {
                    ModelState.AddModelError("", error);
                }
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

        public ActionResult ChangePasswordRequest()
        {
            return View();
        }
    }
}