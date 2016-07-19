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
        public new ActionResult Index()
        {
            return View();
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
                using (AppDbContext db = new AppDbContext())
                {
                    db.accountDb.Add(account);
                    db.SaveChanges();
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
            using (AppDbContext db = new AppDbContext())
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
            using (AppDbContext db = new AppDbContext())
            {
                db.tryLogout();
            }
            return RedirectToAction("Index");
        }
    }
}