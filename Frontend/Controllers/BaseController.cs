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
                BaseControllerSession session = MakeBaseControllerSession(resp);
                return session;
            }
            return null;
        }



        BaseControllerSession MakeBaseControllerSession(TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {
            BaseControllerSession session = new BaseControllerSession();
            session.clientID = resp.clientId;
            session.sessionID = resp.sessionID;
            session.accountSeq = resp.accountSeq;
            session.accountType = resp.accountType;
            session.tradingAccSeq = resp.tradingAccSeq;
            session.tradingAccStatus = resp.tradingAccStatus;
            session.tradingAccList = Newtonsoft.Json.JsonConvert.SerializeObject(resp.tradingAccList);
            return session;
        }


    public void setSession(TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {
            Session["TTLClient"] = resp;
        }
    }
}