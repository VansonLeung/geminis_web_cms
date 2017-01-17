using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static WebApplication2.Controllers.BaseController;

namespace Frontend.Controllers
{
    public class BaseController : Controller
    {
        public BaseControllerSession getSession()
        {
            if (Session["TTLClient"] != null)
            {
                TTLITradeWSDEV.clientLoginResponseLoginResp resp = (TTLITradeWSDEV.clientLoginResponseLoginResp)(Session["TTLClient"]);
                BaseControllerSession session = new BaseControllerSession(
                    resp.clientId,
                    resp.sessionID,
                    resp.accountSeq,
                    resp.accountType,
                    resp.tradingAccSeq,
                    resp.tradingAccStatus
                );
            }
            return null;
        }



        public void setSession(TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {
            Session["TTLClient"] = resp;
        }
    }
}