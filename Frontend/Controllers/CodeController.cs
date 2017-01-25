using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class CodeController : BaseController
    {
        // GET: User
        public ActionResult RegisterEmailCodeCombination()
        {
            // register email and code combination into ip address controller
            // will expire within 15 minutes
            // will not register new code within 5 minute after register, but will refresh existing code's expiry time
            // will register & override with new code after 5 minute after register has passed
            // will also send email to the user
        }

        public bool VerifyEmailCodeCombination(string Email, string Code)
        {
            // find existing non-completed code with correct email + code combination
            // if exists , then complete it, and return true
            // otherwise return false
        }
    }
}