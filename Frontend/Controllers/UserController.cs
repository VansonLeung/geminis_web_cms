using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frontend.Models;
using WebApplication2.Helpers;

namespace Frontend.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Me()
        {

            return null;
        }

        [HttpPost]
        public ActionResult Register(
            string username,
            string email,
            string firstname,
            string lastname,
            string password,
            string tel,
            string otp
            )
        {
            // register and call ttl
            var codeController = new UserCodeController();
            
            if (codeController.VerifyEmailCodeCombination(email, otp))
            {
                TTLAPIRequest form = new TTLAPIRequest(
                    "createClient",
                    new Dictionary<string, object>
                    {
                        ["name"] = firstname,
                        ["cname"] = lastname,
                        ["birthday"] = "1989-01-01",
                        ["idType"] = "H",
                        ["placeOfIssue"] = "HK",
                        ["IDNumber"] = "Y012345(6)",
                        ["sex"] = "M",
                        ["occupationID"] = "1",
                        ["countryOfResidence"] = "HK",
                        ["remark"] = "",
                        ["password"] = password,
                        ["email"] = email,
                        ["mobile"] = tel,
                        ["homeTel"] = tel,
                        ["officeTel"] = tel,
                        ["addressType"] = 1,
                        ["addressTypeSpecified"] = true,
                        ["address1"] = "Room1",
                        ["address2"] = "",
                        ["address3"] = "",
                        ["address4"] = "",
                        ["address5"] = ""
                    }
                );

                var apiController = new APIController();
                var res = apiController.callSoapQuery<TTLITradeWSDEV.createClientResponseCreateClientResp>(form);


                var mailbody = string.Format(
                    "Dear {0} {1}, <br/><br/>" +
                    "<p>Welcome! Your Registration is Complete. <br/>Your client ID is: {2}</p>" +
                    "<p>Geminis CMS Team</p>",
                    firstname, lastname,
                    res.clientID
                );

                var subject = string.Format(
                    "Welcome to Geminis!"
                );

                EmailHelper.SendEmail(new List<string> { email }, mailbody, subject);
                
                return this.Json(BaseResponse.MakeResponse(res));
            }
            return this.Json(BaseResponse.MakeResponse("F001", null, null, "OTP Incorrect"));
        }

        public ActionResult Login()
        {
            // login and call ttl & qpi
            // save session
            // return success 200

            return null;
        }

        public ActionResult KeepAlive()
        {
            // call qpi keepalive api
            // return success 200

            return null;
        }

        public ActionResult Logout()
        {
            // logout and kill session
            // return success 200

            return null;
        }

        public ActionResult EditSettings()
        {
            // what settings:
            // 1. country code
            // 2. lang code
            // 3. locale code

            return null;
        }

        public ActionResult EditProfile()
        {
            // what settings:
            // 1. email
            // 2. password  (call ttl)

            return null;
        }
    }
}