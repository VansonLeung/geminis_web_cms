using Frontend.Attributes;
using Frontend.Bindings;
using Frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Models;
using static Frontend.Models.TTLAPIRequest;
using static WebApplication2.Controllers.BaseController;

namespace Frontend.Controllers
{
    public class SessionController : BaseController
    {
        public class SessionLogin
        {
            public int getSessionKeepaliveMinutes()
            {
                var constant = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SESSION_KEEPALIVE_MINS");
                if (constant != null)
                {
                    return int.Parse(constant.Value);
                }
                return 30;
            }

            public static int getFontSizeNormal()
            {
                return 20;
            }
            public static int getFontSizeBig()
            {
                return 25;
            }

            public int getSessionHeartbeatMinutes()
            {
                var constant = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SESSION_HEARTBEAT_MINS");
                if (constant != null)
                {
                    return int.Parse(constant.Value);
                }
                return 3;
            }

            public string sessionID { get; set; }
            public string userID { get; set; }
            public BaseControllerSession ttlsession { get; set; }
            public string jsessionID { get; set; }
            public DateTime lastLoginDate { get; set; }
            public DateTime keepaliveDate { get; set; }
            public DateTime heartbeatDate { get; set; }
            public bool isForcedExpire { get; set; }

            public bool isKeepaliveDateValid()
            {
                if (isForcedExpire)
                {
                    return false;
                }

                if (keepaliveDate.AddMinutes(getSessionKeepaliveMinutes()) < DateTime.Now)
                {
                    return false;
                }

                return true;
            }

            public bool isHeartbeatDateValid()
            {
                if (isForcedExpire)
                {
                    return false;
                }

                if (heartbeatDate.AddMinutes(getSessionHeartbeatMinutes()) < DateTime.Now)
                {
                    return false;
                }

                return true;
            }


        }

        public static List<SessionLogin> SessionLoginMap = new List<SessionLogin>();
        
        public static SessionLogin GetSessionLoginBySessionID(string sessionID)
        {
            foreach (var sessionLogin in SessionLoginMap)
            {
                if (sessionLogin.sessionID == sessionID)
                {
                    return sessionLogin;
                }
            }
            return null;
        }

        public static List<SessionLogin> GetSessionLoginByUserID(string userID)
        {
            var query = from sessionLogin in SessionLoginMap
                        where sessionLogin.userID == userID
                        select sessionLogin;

            return query.ToList();
        }

        public static SessionLogin UpsertSessionLogin(string sessionID, string userID, BaseControllerSession ttlsession, string jsessionID)
        {
            foreach (var sessionLogin in SessionLoginMap)
            {
                if (sessionLogin.sessionID == sessionID
                    && sessionLogin.userID == userID)
                {
                    sessionLogin.keepaliveDate = DateTime.Now;
                    sessionLogin.heartbeatDate = DateTime.Now;
                    sessionLogin.isForcedExpire = false;
                    sessionLogin.ttlsession = ttlsession;
                    sessionLogin.jsessionID = jsessionID;
                    return sessionLogin;
                }
            }

            SessionLogin sl = new SessionLogin();
            sl.sessionID = sessionID;
            sl.userID = userID;
            sl.keepaliveDate = DateTime.Now;
            sl.heartbeatDate = DateTime.Now;
            sl.isForcedExpire = false;
            sl.ttlsession = ttlsession;
            sl.jsessionID = jsessionID;
            SessionLoginMap.Add(sl);
            return sl;
        }

        public static bool TryKeepaliveSessionLogin(string sessionID, string userID)
        {
            foreach (var sessionLogin in SessionLoginMap)
            {
                if (sessionLogin.sessionID == sessionID
                    && sessionLogin.userID == userID)
                {
                    if (sessionLogin.isKeepaliveDateValid())
                    {
                        sessionLogin.keepaliveDate = DateTime.Now;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool TryHeartbeatSessionLogin(string sessionID, string userID)
        {
            foreach (var sessionLogin in SessionLoginMap)
            {
                if (sessionLogin.sessionID == sessionID
                    && sessionLogin.userID == userID)
                {
                    if (sessionLogin.isHeartbeatDateValid())
                    {
                        sessionLogin.heartbeatDate = DateTime.Now;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckKeepaliveSessionLoginExpired(string sessionID, string userID)
        {
            foreach (var sessionLogin in SessionLoginMap)
            {
                if (sessionLogin.sessionID == sessionID
                    && sessionLogin.userID == userID)
                {
                    if (!sessionLogin.isKeepaliveDateValid())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CheckHeartbeatSessionLoginExpired(string sessionID, string userID)
        {
            foreach (var sessionLogin in SessionLoginMap)
            {
                if (sessionLogin.sessionID == sessionID
                    && sessionLogin.userID == userID)
                {
                    if (!sessionLogin.isHeartbeatDateValid())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CheckForcedSessionLoginExpired(string sessionID, string userID)
        {
            foreach (var sessionLogin in SessionLoginMap)
            {
                if (sessionLogin.sessionID == sessionID
                    && sessionLogin.userID == userID)
                {
                    if (sessionLogin.isForcedExpire)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void ForceAllSessionLoginExpireByUserID(string userID)
        {
            for (var k = SessionLoginMap.Count - 1; k >= 0; k -= 1)
            {
                var sessionLogin = SessionLoginMap[k];
                if (sessionLogin.userID == userID)
                {
                    SessionLoginMap.Remove(sessionLogin);
                }
            }
        }


        public static void ForceSessionLoginExpireBySessionIDAndUserID(string sessionID, string userID)
        {
            for (var k = SessionLoginMap.Count - 1; k >= 0; k -= 1)
            {
                var sessionLogin = SessionLoginMap[k];
                if (sessionLogin.userID == userID
                    && sessionLogin.sessionID == sessionID)
                {
                    SessionLoginMap.Remove(sessionLogin);
                }
            }
        }











        public string GetAPILink()
        {
            var constant = WebApplication2.Context.ConstantDbContext.getInstance().findActiveByKeyNoTracking("DOMAIN_API");
            return constant.Value;
        }



        public ActionResult logout()
        {
            SSO_ClearSession();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult forceExpire(string clientID)
        {
            if (clientID == null)
            {
                clientID = getSession().clientID;
            }
            if (clientID == null)
            {
                return this.Json(BaseResponse.MakeResponse(new Dictionary<string, string>
                {
                }));
            }
            SSO_ForceExpire(clientID);
            return this.Json(BaseResponse.MakeResponse(new Dictionary<string, string>
            {
            }));
        }


        private dynamic ParseHttpGetJson(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    var json = query.Substring(7, query.Length - 7);  // the number 7 is for data=
                    json = System.Web.HttpUtility.UrlDecode(json);
                    dynamic queryJson = JsonConvert.DeserializeObject<dynamic>(json);

                    return queryJson;
                }
                catch (System.Exception e)
                {
                    throw new ApplicationException("wrong json format in the query string！", e);
                }
            }
            else
            {
                return null;
            }
        }


        [ForceApplicationJsonContentType]
        [HttpPost]
        public ActionResult submitSoapQuery()
        {
            try
            {
                String json = new StreamReader(this.Request.InputStream).ReadToEnd();
                TTLAPIRequestForm wrapper = (TTLAPIRequestForm) JsonConvert.DeserializeObject(json, typeof(TTLAPIRequestForm));
                TTLAPIRequest form = wrapper.form;

                // validate form OTP here
                var disable = ConstantDbContext.getInstance().findActiveByKeyNoTracking("INSTRUCTION_FORM_OTP_DISABLE");
                if (disable == null || disable.Value != "1")
                {
                    if (form.otp != null && form.otp != "")
                    {
                        BaseControllerSession session = getSession();

                        string email = session.email;

                        if (!(new UserCodeController().VerifyEmailCodeCombination(email, form.otp)))
                        {
                            return this.Json(BaseResponse.MakeResponse("F002", null, null, "OTP Incorrect"));
                        }
                    }
                }
                var res = new APIController().callSoapQuery<object>(form);
                return this.Json(BaseResponse.MakeResponse(res));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }


        [HttpPost]
        public ActionResult login(string username, string password)
        {
            try
            {
                var res = new APIController().callSoapQuery<TTLITradeWSDEV.clientLoginResponseLoginResp>(
                    new TTLAPIRequest(
                        "clientLogin",
                        new Dictionary<string, object>
                        {
                            ["ChannelID"] = "INT",
                            ["ClientID"] = username,
                            ["Password"] = password,
                            ["TradingAccSeq"] = "-1",
                            ["Encrypt"] = "Y",
                        })
                );

                //TTLITradeWSDEV.ItradeWebServicesClient soap = new TTLITradeWSDEV.ItradeWebServicesClient();
                var resp = (TTLITradeWSDEV.clientLoginResponseLoginResp)res;

                if (resp.errorCode != null || resp.errorMessage != null)
                {
                    return this.Json(BaseResponse.MakeResponse("F001", resp.errorCode, null, resp.errorMessage));
                }



                setSession(resp);


                BaseControllerSession session = getSession();


                if (session != null)
                {
                    try
                    {
                        var isNonTradingAccField = "1";
                        if (session.hasTradingAcc)
                        {
                            isNonTradingAccField = "-1";
                        }

                        var res2 = new APIController().callSoapQuery<TTLITradeWSDEV.queryAccountDetailsResponseQueryAccountDetailsResp>(
                            new TTLAPIRequest(
                                "queryAccountDetails",
                                new Dictionary<string, object>
                                {
                                    ["ClientID"] = session.clientID,
                                    ["SessionID"] = session.sessionID,
                                    ["isNonTradingAccField"] = isNonTradingAccField,
                                    ["version"] = "1",
                                    ["deviceID"] = "",
                                    ["osVersion"] = "1",
                                })
                        );

                        if (res2 != null)
                        {
                            setAccSession(res2);
                        }
                    }
                    catch (Exception e)
                    {
                        AuditLogDbContext.getInstance().createAuditLog(new AuditLog
                        {
                            action = "queryAccountDetails",
                            remarks = "failed",
                        });
                    }
                }


                /*
                var jsession = loginQPI(username, password, resp);

                if (jsession.Result != null)
                {
                    setJSession(jsession.Result);
                }
                */

                var is_sso_enabled = false;
                var sso_enabled = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SSO_enabled");
                if (sso_enabled != null
                    && sso_enabled.Value != null)
                {
                    is_sso_enabled = sso_enabled.Value == "1";
                }

                SSO_UpsertUser(is_sso_enabled);

                return this.Json(BaseResponse.MakeResponse(resp));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }

        [HttpPost]
        public ActionResult keepalive_ttl(string ClientID, string SessionID)
        {
            try
            {
                var res = new APIController().callSoapQuery<TTLITradeWSDEV.isClientLoginResponseIsLoginResp>(
                    new TTLAPIRequest(
                        "isClientLogin",
                        new Dictionary<string, object>
                        {
                            ["ClientID"] = ClientID,
                            ["SessionID"] = SessionID,
                        })
                );

                //TTLITradeWSDEV.ItradeWebServicesClient soap = new TTLITradeWSDEV.ItradeWebServicesClient();
                var resp = (TTLITradeWSDEV.isClientLoginResponseIsLoginResp)res;

                if (resp.errorCode != null || resp.errorMessage != null)
                {
                    return this.Json(BaseResponse.MakeResponse("F001", resp.errorCode, null, resp.errorMessage));
                }

                return this.Json(BaseResponse.MakeResponse(resp));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }

        [HttpPost]
        public bool keepalive_ttl_internal(string ClientID, string SessionID)
        {
            if (ClientID == null || SessionID == null)
            {
                return false;
            }

            try
            {
                var res = new APIController().callSoapQuery<TTLITradeWSDEV.isClientLoginResponseIsLoginResp>(
                    new TTLAPIRequest(
                        "isClientLogin",
                        new Dictionary<string, object>
                        {
                            ["ClientID"] = ClientID,
                            ["SessionID"] = SessionID,
                        })
                );

                //TTLITradeWSDEV.ItradeWebServicesClient soap = new TTLITradeWSDEV.ItradeWebServicesClient();
                var resp = (TTLITradeWSDEV.isClientLoginResponseIsLoginResp)res;

                if (resp.errorCode != null || resp.errorMessage != null)
                {
                    AuditLogDbContext.getInstance().createAuditLog(new AuditLog
                    {
                        is_private = true,
                        action = "TTL INTERNAL",
                        remarks = "FAIL KEEP ALIVE (" + ClientID + ")",
                    });
                    return false;
                }

                AuditLogDbContext.getInstance().createAuditLog(new AuditLog
                {
                    is_private = true,
                    action = "TTL INTERNAL",
                    remarks = "SUCCESS KEEP ALIVE (" + ClientID + ")",
                });
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }



        [HttpPost]
        public ActionResult get_qpi_login_params()
        {
            /* QPI (Client side) */
            BaseControllerSession session = getSession();

            if (session == null)
            {
                return this.Json(BaseResponse.MakeResponse(new Dictionary<string, string>
                {
                }));
            }

            string login_url = "";
            string keep_alive_url = "";
            var qpisession = getSession();
            string jsessionid = null;
            if (qpisession != null)
            {
                jsessionid = qpisession.jsessionID;
            }

            login_url = "http://uat.quotepower.com/web/geminis/login.jsp";
            var _constant = ConstantDbContext.getInstance().findActiveByKeyNoTracking("IFRAME_QPI_URL");
            if (_constant != null)
            {
                login_url = _constant.Value + "login.jsp";
            }


            keep_alive_url = "http://uat.quotepower.com/web/luso/json/heartbeat.jsp";
            _constant = ConstantDbContext.getInstance().findActiveByKeyNoTracking("IFRAME_QPI_LUSO");
            if (_constant != null)
            {
                keep_alive_url = _constant.Value + "json/heartbeat.jsp";
            }

            string domain = "GEMINIS";
            string uid = session.clientID;
            string ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            string env_key = "UAT";
            string password = "d6sd$#sf";

            var enqstr = "";
            enqstr += "domain=" + domain;
            enqstr += "&uid=" + uid;
            enqstr += "&password=" + password;
            enqstr += "&ts=" + ts;
            enqstr += "&env_key=" + env_key;

            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(enqstr), 0, Encoding.UTF8.GetByteCount(enqstr));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            string hashstr = hash.ToString();

            return this.Json(BaseResponse.MakeResponse(new Dictionary<string, string> {
                ["domain"] = domain,
                ["uid"] = uid,
                ["ts"] = ts,
                ["env_key"] = env_key,
                ["token"] = hashstr,
                ["password"] = password,
                ["login_url"] = login_url,
                ["keep_alive_url"] = keep_alive_url,
                ["jsessionid"] = jsessionid
            }));
        }


        [ForceApplicationJsonContentType]
        [HttpPost]
        public ActionResult set_qpi_login_token(QPIAPIResponse jsession)
        {
            setJSession(jsession);
            return this.Json(BaseResponse.MakeResponse("true"));
        }


        public async Task<QPIAPIResponse> loginQPI(string username, string password, TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {

            /* QPI */

            HttpClient client = new HttpClient();

            string domain = "GEMINIS";
            string uid = username;
            string ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            string env_key = "UAT";

            var enqstr = "";
            enqstr += "domain=" + domain;
            enqstr += "&uid=" + uid;
            enqstr += "&password=" + "d6sd$#sf";
            enqstr += "&ts=" + ts;
            enqstr += "&env_key=" + env_key;

            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(enqstr), 0, Encoding.UTF8.GetByteCount(enqstr));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            string hashstr = hash.ToString();



            var query = HttpUtility.ParseQueryString(string.Empty);
            query["domain"] = domain;
            query["uid"] = uid;
            query["ts"] = ts;
            query["token"] = hashstr;
            string queryString = query.ToString();

            client.BaseAddress = new Uri("http://uat.quotepower.com/web/geminis/login.jsp");

            // List data response.
            HttpResponseMessage response = client.GetAsync("?" + queryString).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var qpiapiresponse = JsonConvert.DeserializeObject<QPIAPIResponse>(json);

                return qpiapiresponse;

                // Parse the response body. Blocking!
                /*
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;
                foreach (var d in dataObjects)
                {
                    Console.WriteLine("{0}", d.Name);
                }
                */
            }
            return null;
        }




        public ActionResult keepAlive()
        {
            Session["keepaliveTime"] = DateTime.Now;

            if (Session["jsessionID"] != null)
            {
                string jsessionID = (string) Session["jsessionID"];
                bool success = keepAliveQPI(jsessionID);
                Session["jsessionIDkeepAlive"] = success;
            }
            return null;
        }





        [ForceApplicationJsonContentType]
        [HttpPost]
        public ActionResult api_sso_keepalive()
        {
            return this.Json(SSO_InternalKeepAlive());
        }


        [ForceApplicationJsonContentType]
        [HttpPost]
        public ActionResult api_sso_heartbeat()
        {
            return this.Json(SSO_InternalHeartbeat());
        }

        
        [ForceApplicationJsonContentType]
        [HttpPost]
        public ActionResult api_sso_force_expire(string clientID)
        {
            return this.Json(forceExpire(clientID));
        }





        public bool keepAliveQPI(string jsessionID)
        {

            /* QPI */

            HttpClient client = new HttpClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["jsessionid"] = jsessionID;
            string queryString = query.ToString();
            
            // List data response.
            client.BaseAddress = new Uri("http://uat.quotepower.com/web/luso/json/heartbeat.jsp");

            // List data response.
            HttpResponseMessage response = client.GetAsync(";" + queryString).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}