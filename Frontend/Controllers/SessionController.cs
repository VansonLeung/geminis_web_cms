using Frontend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class SessionController : BaseController
    {
        public string GetAPILink()
        {
            var constant = WebApplication2.Context.ConstantDbContext.getInstance().findActiveByKeyNoTracking("DOMAIN_API");
            return constant.Value;
        }


        [HttpPost]
        public ActionResult login(string username, string password)
        {
            try
            {
                var res = new APIController().callSoapQuery(
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
                    return this.Json(BaseResponse.MakeResponse("F001", resp.errorCode, resp, resp.errorMessage));
                }

                setSession(resp);

                var jsession = loginQPI(username, password, resp);

                setJSession(jsession);

                return this.Json(BaseResponse.MakeResponse(resp));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }


        public string loginQPI(string username, string password, TTLITradeWSDEV.clientLoginResponseLoginResp resp)
        {

            /* QPI */

            HttpClient client = new HttpClient();

            string domain = "GEMINIS";
            string uid = username;
            string password = password;
            string ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            string env_key = "UAT";

            var enq = HttpUtility.ParseQueryString(string.Empty);
            enq["domain"] = domain;
            enq["uid"] = uid;
            enq["password"] = password;
            enq["ts"] = ts;
            enq["env_key"] = env_key;
            string enqstr = enq.ToString();

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

            client.BaseAddress = new Uri("http://uat.quotepower.com/web/geminis/login.jsp?" + queryString);

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;
                foreach (var d in dataObjects)
                {
                    Console.WriteLine("{0}", d.Name);
                }
            }

            return "ABC";
        }



        public ActionResult keepAlive()
        {
            if (Session["jsessionID"] != null)
            {
                string jsessionID = (string) Session["jsessionID"];
                keepAliveQPI(jsessionID);
            }
            return null;
        }



        public bool keepAliveQPI(string jsessionID)
        {

            /* QPI */

            HttpClient client = new HttpClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["jsessionid"] = jsessionID;
            string queryString = query.ToString();

            client.BaseAddress = new Uri("http://uat.quotepower.com/web/luso/json/heartbeat.jsp;" + queryString);

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}