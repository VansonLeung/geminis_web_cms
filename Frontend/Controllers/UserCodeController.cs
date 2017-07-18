using WebApplication2.Context;
using WebApplication2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Helpers;
using Frontend.Models;
using static WebApplication2.Controllers.BaseController;

namespace Frontend.Controllers
{
    public class UserCodeController : BaseController
    {
        // POST: Apply for email code
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }


        [HttpPost]
        public ActionResult RegisterAnonymous(string email)
        {
            // register email and code combination into ip address controller
            // will expire within 15 minutes
            // will not register new code within 5 minute after register, but will refresh existing code's expiry time
            // will register & override with new code after 5 minute after register has passed
            // will also send email to the user

            try
            {
                var res = RegisterEmailCodeCombination(email);
                
                return this.Json(BaseResponse.MakeResponse(res));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }


        [HttpPost]
        public ActionResult RegisterFromSession()
        {
            // register email and code combination into ip address controller
            // will expire within 15 minutes
            // will not register new code within 5 minute after register, but will refresh existing code's expiry time
            // will register & override with new code after 5 minute after register has passed
            // will also send email to the user

            try
            {
                BaseControllerSession session = getSession();

                string email = session.email;

                var res = RegisterEmailCodeCombination(email);

                return this.Json(BaseResponse.MakeResponse(res));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }


        [HttpPost]
        public ActionResult VerifyAnonymous(string email, string code)
        {
            // register email and code combination into ip address controller
            // will expire within 15 minutes
            // will not register new code within 5 minute after register, but will refresh existing code's expiry time
            // will register & override with new code after 5 minute after register has passed
            // will also send email to the user

            try
            {
                var res = VerifyEmailCodeCombination(email, code);

                return this.Json(BaseResponse.MakeResponse(res));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }
        

        public bool RegisterEmailCodeCombination(string Email)
        {
            var db = UserCodeDbContext.getInstance().getItemDb();

            var code = new UserCode();
            code.Email = Email;
            code.Key = "" + GenerateRandomNo();
            code.RegisteredAt = DateTime.Now;

            db.Add(code);
            UserCodeDbContext.getInstance().db.SaveChanges();

            var mailbody = string.Format(
                "Dear {0}, <br/><br/>" +
                "<p>Here is your registration verification code: {1}</p>" +
                "<p>Geminis CMS Team</p>",
                code.Email,
                code.Key
            );

            var subject = string.Format(
                "[Geminis] Registration Verification Code"
            );

            

            EmailHelper.SendEmail(new List<string> { code.Email }, mailbody, subject);

            return true;
        }

        public bool VerifyEmailCodeCombination(string Email, string Code)
        {
            var db = UserCodeDbContext.getInstance().db;
            var itemDb = UserCodeDbContext.getInstance().getItemDb();

            // find existing non-completed code with correct email + code combination
            // if exists , then complete it, and return true
            // otherwise return false

            var codes = itemDb.Where(c => c.Email == Email && c.Key == Code).ToList();
            if (codes.Count <= 0)
            {
                return false;
            }
            foreach (var code in codes)
            {
                //if (code.isExpired())
                //{
                //    continue;
                //}
                
                if (code.Completed)
                {
                    continue;
                }
                
                
                db.Entry(code).State = EntityState.Modified;
                code.Completed = true;
                db.SaveChanges();

                return true;
            }

            return false;
        }
    }
}