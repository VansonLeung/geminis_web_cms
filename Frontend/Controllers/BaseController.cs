using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
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

                if (Session["jsessionID"] != null)
                {
                    string jsessionID = (string)Session["jsessionID"];
                    session.jsessionID = jsessionID;
                }

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

        public void setJSession(QPIAPIResponse resp)
        {
            Session["jsessionID"] = resp.session;
            Session["jsessionIDdateDT"] = resp.header.dateDT;
        }




        public bool SessionTimeout()
        {
            if (Session["keepaliveTime"] != null)
            {
                DateTime keepaliveTime = (DateTime)Session["keepaliveTime"];
                DateTime now = DateTime.Now;
                var minutes = (now - keepaliveTime).TotalMinutes;
                if (minutes > 30)
                {
                    return true;
                }
            }

            return false;
        }


        public void ClearSession()
        {
            Session.Abandon();
        }


        public bool InternalKeepAlive()
        {
            Session["keepaliveTime"] = DateTime.Now;

            if (Session["jsessionID"] != null)
            {
                string jsessionID = (string)Session["jsessionID"];
                var sessionController = new SessionController();
                bool success = sessionController.keepAliveQPI(jsessionID);
                Session["jsessionIDkeepAlive"] = success;
                return success;
            }
            return false;
        }
    }
}