using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class CodeController : BaseController
    {
        // POST: Apply for email code
        public ActionResult RegisterEmailCodeCombination()
        {
            // register email and code combination into ip address controller
            // will expire within 15 minutes
            // will not register new code within 5 minute after register, but will refresh existing code's expiry time
            // will register & override with new code after 5 minute after register has passed
            // will also send email to the user

            Code = new Code()
            code.email = params.email;
            code.code = Random(1234567890)Random(1234567890)Random(1234567890)Random(1234567890)
            code.createdAt = new Date()

            db.Save(code);

            var mailbody = string.Format(
                "Dear {0}, <br/><br/>" +
                "<p>Here is your registration verification code: {1}</p>" +
                "<p>Geminis CMS Team</p>",
                code.email,
                code.code
            );

            var subject = string.Format(
                "[Geminis] Registration Verification Code"
            );

            EmailHelper.SendEmail(new List<string> { code.Email }, mailbody, subject);

            return null;
        }

        public bool VerifyEmailCodeCombination(string Email, string Code)
        {
            // find existing non-completed code with correct email + code combination
            // if exists , then complete it, and return true
            // otherwise return false

            var codes = db.where(c => c.email == Email && c.code == Code).List();
            if (codes.Count <= 0)
            {
                return false;
            }
            foreach (var code in codes)
            {
                if (code.isExpired())
                {
                    continue;
                }
                
                if (code.isUsed())
                {
                    continue;
                }

                code.isUsed = true;
                code.save();

                return true;
            }

            return false;
        }
    }
}