using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public virtual ActionResult Index()
        {
            return View();
        }



        public class BaseControllerSession
        {
            public bool isExpiredAlready { get; set; }
            public string clientID { get; set; }
            public string sessionID { get; set; }
            public int accountSeq { get; set; }
            public string accountType { get; set; }
            public string tradingAccSeq { get; set; }
            public string tradingAccStatus { get; set; }
            public string tradingAccList { get; set; }
            public string jsessionID { get; set; }
            public string fullname { get; set; }
        }
    }
}