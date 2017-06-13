using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using static Frontend.Controllers.SessionController;
using static WebApplication2.Controllers.BaseController;

namespace Frontend.Controllers
{
    public class BaseController : Controller
    {
        public BaseControllerSession getSession(bool excludePostLogin = false)
        {
            BaseControllerSession session = new BaseControllerSession();
            session.isLoggedIn = false;

            if (!excludePostLogin)
            {
                if (Session["TTLClient"] != null)
                {
                    TTLITradeWSDEV.clientLoginResponseLoginResp resp = (TTLITradeWSDEV.clientLoginResponseLoginResp)(Session["TTLClient"]);
                    session = MakeBaseControllerSession(resp);
                    session.isLoggedIn = true;

                    if (Session["TTLAccount"] != null)
                    {
                        TTLITradeWSDEV.queryAccountDetailsResponseQueryAccountDetailsResp resp2 = (TTLITradeWSDEV.queryAccountDetailsResponseQueryAccountDetailsResp)(Session["TTLAccount"]);
                        session.email = resp2.email;
                    }

                    if (Session["jsessionID"] != null)
                    {
                        string jsessionID = (string)Session["jsessionID"];
                        session.jsessionID = jsessionID;
                    }
                }
            }

            if (excludePostLogin || !session.isLoggedIn)
            {
                Session["isKeptAlive"] = false;
            }

            session.fontSize = SessionLogin.getFontSizeNormal();
            if (Session["fontSize"] != null)
            {
                session.fontSize = (int)Session["fontSize"];
            }

            if (Session["isKeptAlive"] != null)
            {
                session.isKeptAlive = (bool)Session["isKeptAlive"];
            }
            else
            {
                session.isKeptAlive = false;
            }

            return session;
        }
        


        BaseControllerSession MakeBaseControllerSession(TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {
            BaseControllerSession session = new BaseControllerSession();
            session.fullname = resp.fullname;
            session.clientID = resp.clientId;
            session.sessionID = resp.sessionID;
            session.accountSeq = resp.accountSeq;
            session.accountType = resp.accountType;
            session.tradingAccSeq = resp.tradingAccSeq;
            session.tradingAccStatus = resp.tradingAccStatus;
            session.tradingAccList = Newtonsoft.Json.JsonConvert.SerializeObject(resp.tradingAccList);
            if (resp.tradingAccList != null && resp.tradingAccList.Length > 0)
            {
                var acc = resp.tradingAccList[0];
                if (acc != null)
                {
                    session.hasTradingAcc = true;
                    session.ttL_accountSeqField = acc.accountSeq;
                    session.ttL_accountTypeField = acc.accountType;
                    session.ttL_defaultSubAccountField = acc.defaultSubAccount;
                    session.ttL_investorTypeIDField = acc.investorTypeID;
                    session.ttL_tradingAccSeqField = acc.tradingAccSeq;
                    session.ttL_tradingAccStatusField = acc.tradingAccStatus;
                }
            }
            return session;
        }


        public void setSession(TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {
            Session["TTLClient"] = resp;
        }

        public void setAccSession(TTLITradeWSDEV.queryAccountDetailsResponseQueryAccountDetailsResp resp)
        {
            Session["TTLAccount"] = resp;
        }

        public void setJSession(QPIAPIResponse resp)
        {
            if (resp == null)
            {
                Session["jsessionID"] = null;
                Session["jsessionIDdateDT"] = null;
            }
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
            setSession(null);
            setAccSession(null);
            setJSession(null);
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








        public string GetCurrentSessionID()
        {
            return System.Web.HttpContext.Current.Session.SessionID;
        }








        public void SSO_UpsertUser(bool isForceExpireAll = false)
        {
            var ttlsession = getSession();
            var sessionID = GetCurrentSessionID();
            string userID = null;
            if (ttlsession != null && ttlsession.clientID != null)
            {
                userID = ttlsession.clientID;
            }

            if (sessionID != null && userID != null)
            {
                if (isForceExpireAll)
                {
                    SessionController.ForceAllSessionLoginExpireByUserID(userID);
                }

                string jsessionID = null;
                if (Session["jsessionID"] != null)
                {
                    jsessionID = (string)Session["jsessionID"];
                }

                SessionController.UpsertSessionLogin(sessionID, userID, ttlsession, jsessionID);
            }
        }


        public void SSO_ForceExpire(string userID)
        {
            SessionController.ForceAllSessionLoginExpireByUserID(userID);
        }



        public bool SSO_SessionTimeout()
        {
            var ttlsession = getSession();
            var sessionID = GetCurrentSessionID();
            string userID = null;
            if (ttlsession != null && ttlsession.clientID != null)
            {
                userID = ttlsession.clientID;
            }

            if (sessionID != null && userID != null)
            {
                if (SessionController.CheckKeepaliveSessionLoginExpired(sessionID, userID)
                    || SessionController.CheckHeartbeatSessionLoginExpired(sessionID, userID)
                    || SessionController.CheckForcedSessionLoginExpired(sessionID, userID))
                {
                    return true;
                }
            }

            return false;
        }


        public void SSO_ClearSession()
        {
            var ttlsession = getSession();
            var sessionID = GetCurrentSessionID();
            string userID = null;
            if (ttlsession != null && ttlsession.clientID != null)
            {
                userID = ttlsession.clientID;
            }

            if (sessionID != null && userID != null)
            {
                SessionController.ForceSessionLoginExpireBySessionIDAndUserID(sessionID, userID);
            }

            Session.Abandon();
        }


        public bool SSO_InternalKeepAlive()
        {
            var ttlsession = getSession();
            var sessionID = GetCurrentSessionID();
            string userID = null;
            if (ttlsession != null && ttlsession.clientID != null)
            {
                userID = ttlsession.clientID;
            }

            if (sessionID != null && userID != null)
            {
                if (ttlsession != null && ttlsession.clientID != null && ttlsession.sessionID != null)
                {
                    // TODO: KEEP ALIVE TTL SHOULD BLOCK LOGIN...
                    new SessionController().keepalive_ttl_internal(ttlsession.clientID, ttlsession.sessionID);
                }

                SessionController.TryKeepaliveSessionLogin(sessionID, userID);

                if (Session["jsessionID"] != null)
                {
                    string jsessionID = (string)Session["jsessionID"];
                    var sessionController = new SessionController();
                    bool success = sessionController.keepAliveQPI(jsessionID);
                    Session["jsessionIDkeepAlive"] = success;
                    return success;
                }
            }
            return false;
        }


        public bool SSO_InternalHeartbeat()
        {
            var ttlsession = getSession();
            var sessionID = GetCurrentSessionID();
            string userID = null;
            if (ttlsession != null && ttlsession.clientID != null)
            {
                userID = ttlsession.clientID;
            }

            if (sessionID != null && userID != null)
            {
                SessionController.TryHeartbeatSessionLogin(sessionID, userID);
            }
            return false;
        }




        public void SetFontSizeNormal()
        {
            Session["fontSize"] = SessionLogin.getFontSizeNormal();
        }

        public void SetFontSizeBig()
        {
            Session["fontSize"] = SessionLogin.getFontSizeBig();
        }
    }
}