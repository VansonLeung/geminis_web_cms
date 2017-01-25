﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Me()
        {
            return me
        }

        public ActionResult Register()
        {
            // register and call ttl
        }

        public ActionResult Login()
        {
            // login and call ttl & qpi
            // save session
            // return success 200
        }

        public ActionResult KeepAlive()
        {
            // call qpi keepalive api
            // return success 200
        }

        public ActionResult Logout()
        {
            // logout and kill session
            // return success 200
        }

        public ActionResult EditSettings()
        {
            // what settings:
            // 1. country code
            // 2. lang code
            // 3. locale code
        }

        public ActionResult EditProfile()
        {
            // what settings:
            // 1. email
            // 2. password  (call ttl)
        }
    }
}