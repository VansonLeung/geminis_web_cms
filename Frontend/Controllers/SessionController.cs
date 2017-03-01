using Frontend.Attributes;
using Frontend.Bindings;
using Frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using static Frontend.Models.TTLAPIRequest;

namespace Frontend.Controllers
{
    public class SessionController : BaseController
    {
        public string GetAPILink()
        {
            var constant = WebApplication2.Context.ConstantDbContext.getInstance().findActiveByKeyNoTracking("DOMAIN_API");
            return constant.Value;
        }


    
        [ForceApplicationJsonContentType]
        [HttpPost]
        public ActionResult submitSoapQuery(TTLAPIRequestForm wrapper)
        {
            try
            {
                // validate form OTP here
                TTLAPIRequest form = wrapper.form;

                if (form.otp != null && form.otp != "")
                {
                    if (!(new UserCodeController().VerifyEmailCodeCombination("kaycheung@cherrypicks.com", form.otp)))
                    {
                        return this.Json(BaseResponse.MakeResponse("F002", null, null, "OTP Incorrect"));
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
                    return this.Json(BaseResponse.MakeResponse("F001", resp.errorCode, resp, resp.errorMessage));
                }

                setSession(resp);

                var jsession = loginQPI(username, password, resp);

                if (jsession.Result != null)
                {
                    setJSession(jsession.Result);
                }

                return this.Json(BaseResponse.MakeResponse(resp));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
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