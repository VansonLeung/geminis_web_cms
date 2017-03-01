using Frontend.Bindings;
using Frontend.Models;
using Frontend.TTLITradeWSDEV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Frontend.Controllers
{
    public class APIController : BaseController
    {
        public ActionResult Index()
        {
            return this.Json("OK");
        }

/*
        public BaseRequest_CType getRequest(string name)
        {
            TTLITradeWSDEV.
            TTLITradeWSDEV.stockPriceInfoStockPriceInfoReq req;
            req.clientID;
            req.deviceID;
            req.instrumentID;
            BaseRequest_CType g;
            g.in
        }
*/




        [JSONParamBinding]
        [HttpPost]
        public ActionResult submitSoapQuery(TTLAPIRequest form)
        {
            try
            {
                // validate form OTP here
                
                if (form.otp != null && form.otp != "")
                {
                    if (!(new UserCodeController().VerifyEmailCodeCombination("kay@cherrypicks.com", form.otp)))
                    {
                        return this.Json(BaseResponse.MakeResponse("F002", null, null, "OTP Incorrect"));
                    }
                }

                var res = callSoapQuery<object>(form);
                return this.Json(BaseResponse.MakeResponse(res));
            }
            catch (Exception e)
            {
                return this.Json(BaseResponse.MakeResponse("F001", e));
            }
        }







        public T callSoapQuery<T>(TTLAPIRequest form)
        {

            TTLITradeWSDEV.ItradeWebServicesClient soap = new TTLITradeWSDEV.ItradeWebServicesClient();

            TTLITradeWSDEV.requestHeaderType reqHeader = new TTLITradeWSDEV.requestHeaderType();
            TTLITradeWSDEV.responseHeaderType respHeader = new TTLITradeWSDEV.responseHeaderType();

            TTLITradeWSDEV.BaseRequest_CType query = null;
            TTLITradeWSDEV.BaseResponse_CType response = null;


            if (form.name != null)
            {
                MethodInfo mth = soap.GetType().GetMethod(form.name);
                ParameterInfo[] pms = mth.GetParameters();

                int i = 0;
                foreach (ParameterInfo _param in pms)
                {
                    var fullName = _param.ParameterType.FullName;
                    fullName = fullName.Replace("&", "");

                    Type type = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.FullName == fullName);

                    var obj = Activator.CreateInstance(type);

                    if (typeof(requestHeaderType).IsAssignableFrom(obj.GetType()))
                    {
                        reqHeader = (requestHeaderType)obj;
                    }
                    if (typeof(BaseRequest_CType).IsAssignableFrom(obj.GetType()))
                    {
                        query = (BaseRequest_CType)obj;
                    }
                    if (typeof(BaseResponse_CType).IsAssignableFrom(obj.GetType()))
                    {
                        response = (BaseResponse_CType)obj;
                    }
                    i++;
                }
            }

            soap.ClientCredentials.UserName.UserName = form.credentials.username;
            soap.ClientCredentials.UserName.Password = form.credentials.password;

            reqHeader.version = form.header.version;
            reqHeader.traceNo = form.header.traceNo;


            if (form.body != null)
            {
                string json = JsonConvert.SerializeObject(form.body, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
                query = (BaseRequest_CType)JsonConvert.DeserializeObject(json, query.GetType());

                /*
                var keys = form.body.Keys;
                foreach (string key in keys)
                {
                    var val = form.body[key];
                    if (val != null)
                    {

                        var property = query.GetType().GetProperty(key);
                        if (property == null)
                        {
                            continue;
                        }

                        if (property.PropertyType == typeof(string)
                            || property.PropertyType == typeof(int)
                            || property.PropertyType == typeof(bool)
                            || property.PropertyType == typeof(decimal)
                            || property.PropertyType == typeof(float)
                            || property.PropertyType == typeof(double))
                        {
                            property.SetValue(query, Convert.ChangeType(val, property.PropertyType), null);
                        }
                        else
                        {
                            object ob = Newtonsoft.Json.JsonConvert.DeserializeObject<property.PropertyType>(Json Object);
                            property.SetValue(query, )
                        }
                    }
                }
                */
            }

            try
            {
                MethodInfo mth = soap.GetType().GetMethod(form.name);
                List<object> parameters = new List<object>
                {
                    reqHeader,
                    query,
                    null
                };

                var p = parameters.ToArray();
                try
                {
                    object resp = mth.Invoke(soap, p);
                    if (respHeader.GetType().IsAssignableFrom(resp.GetType()))
                    {
                        respHeader = (responseHeaderType)resp;
                    }

                    T res = (T)p[2];
                    return res;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }








        private DateTime BuildDateTimeFromYAFormat(string dateString)
        {
            Regex r = new Regex(@"^\d{4}\d{2}\d{2}T\d{2}\d{2}\d{2}$");
            if (!r.IsMatch(dateString))
            {
                throw new FormatException(
                    string.Format("{0} is not the correct format. Should be yyyyMMddThhmmss", dateString));
            }

            DateTime dt = DateTime.ParseExact(dateString, "yyyyMMddThhmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

            return dt;
        }
    }
}