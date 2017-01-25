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
        }

        public ActionResult RegisterIPAddressFailLoginRetries()
        {
            // register ip address fail login retries once
        }

        public int GetIPAddressStatus()
        {
            // get ip validity
            // if login retries > N times
            // return 403 (suspended)
            // return 200 (OK)
        }

        public ActionResult ClearFails()
        {
        	// admin access only
        }
    }
}