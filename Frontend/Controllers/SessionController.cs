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

                return this.Json(BaseResponse.MakeResponse(resp));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }
    }
}