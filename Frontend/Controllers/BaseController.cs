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
            if (Session["TTLClient"] != null
                && Session["jsessionID"] != null)
            {
                TTLITradeWSDEV.clientLoginResponseLoginResp resp = (TTLITradeWSDEV.clientLoginResponseLoginResp)(Session["TTLClient"]);
                string jsessionID = (string) Session["jsessionID"];
                BaseControllerSession session = MakeBaseControllerSession(resp, jsessionID);
                return session;
            }
            return null;
        }



        BaseControllerSession MakeBaseControllerSession(TTLITradeWSDEV.clientLoginResponseLoginResp resp, string jsessionID)
        {
            BaseControllerSession session = new BaseControllerSession();
            session.clientID = resp.clientId;
            session.sessionID = resp.sessionID;
            session.accountSeq = resp.accountSeq;
            session.accountType = resp.accountType;
            session.tradingAccSeq = resp.tradingAccSeq;
            session.tradingAccStatus = resp.tradingAccStatus;
            session.tradingAccList = Newtonsoft.Json.JsonConvert.SerializeObject(resp.tradingAccList);
            session.jsessionID = jsessionID;
            return session;
        }


        public void setSession(TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {
            Session["TTLClient"] = resp;
        }

        public void setJSession(QPIAPIResponse resp)
        {
            Session["jsessionID"] = resp.session;
            Session["jsessionIDdateDT"] = resp.header.dateDT;
        }
    }
}