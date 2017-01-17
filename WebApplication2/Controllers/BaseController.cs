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
            public BaseControllerSession(
                string clientID,
                string sessionID,
                int accountSeq,
                string accountType,
                string tradingAccSeq,
                string tradingAccStatus)
            {
                this.clientID = clientID;
                this.sessionID = sessionID;
                this.accountSeq = accountSeq;
                this.accountType = accountType;
                this.tradingAccSeq = tradingAccSeq;
                this.tradingAccStatus = tradingAccStatus;
            }
            public string clientID { get; set; }
            public string sessionID { get; set; }
            public int accountSeq { get; set; }
            public string accountType { get; set; }
            public string tradingAccSeq { get; set; }
            public string tradingAccStatus { get; set; }
        }
    }
}