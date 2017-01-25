using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class IPAddressController : BaseController
    {
    	int loginRetries = 5;
        // GET: User
        public ActionResult RegisterIPAddress()
        {
            // register ip address

            return null;
        }

        public ActionResult RegisterIPAddressFailLoginRetries()
        {
            // register ip address fail login retries once

            return null;
        }

        public int GetIPAddressStatus()
        {
            // get ip validity
            // if login retries > N times
            // return 403 (suspended)
            // return 200 (OK)

            return 0;
        }

        public ActionResult ClearFails()
        {
            // admin access only

            return null;
        }
    }
}