using API.Bindings;
using API.Models;
using API.TTLITradeWSDEV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Script.Serialization;

namespace API.Controllers
{
    public class APIController : ApiController
    {
        [HttpGet]
        [Route("demo/get_list")]
        public IHttpActionResult DemoGetList()
        {
            List<Movie> movies = new List<Movie>()
            {
                new Movie()
                {
                    Id = 1,
                    Title = "Up",
                    ReleaseDate = new DateTime(2009,5,29),
                    RunningTimeMinutes = 96
                },
                new Movie()
                {
                    Id = 2,
                    Title = "Toy Story",
                    ReleaseDate = new DateTime(1995, 11, 19),
                    RunningTimeMinutes = 81
                },
                new Movie()
                {
                    Id = 3,
                    Title = "Big Hero 6",
                    ReleaseDate = new DateTime(2014, 11, 7),
                    RunningTimeMinutes = 102
                }
            };

            return Ok(BaseResponse.MakeResponse(movies));
        }

        [HttpGet]
        [Route("demo/get_item/{id}")]
        public IHttpActionResult DemoGetItem(int id)
        {
            return Ok(BaseResponse.MakeResponse(new Movie()
            {
                Id = id,
                Title = "Big Hero 6",
                ReleaseDate = new DateTime(2014, 11, 7),
                RunningTimeMinutes = 102
            }));
        }


        [JSONParamBinding]
        [HttpPost]
        [Route("demo/submit_form_1")]
        public IHttpActionResult DemoSubmitForm1(RequestForm1 param)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            if (param.a == null || param.a.Equals("")) { errors.Add("a", "F001"); };
            if (param.b == null || param.b.Equals("")) { errors.Add("b", "F001"); };
            if (param.c == 0) { errors.Add("c", "F001"); };
            if (param.date == null || param.date.Equals("")) { errors.Add("date", "F001"); };

            DateTime? dateTime = null;
            String recvDate = "";

            try
            {
                dateTime = BuildDateTimeFromYAFormat(param.date);
                if (dateTime != null)
                {
                    recvDate = dateTime.GetValueOrDefault().ToString();
                }
            } catch (FormatException e)
            {

            }


            if (errors.Count > 0)
            {
                return Ok(BaseResponse.MakeResponse("F000", errors));
            }

            var _params = GetType().GetMethod("submitSoapQuery_accountBalanceEnquiry").GetParameters();
            var paramStr = "";
            foreach (ParameterInfo _param in _params)
            {
                paramStr += _param.ParameterType;
                
                Type type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == _param.ParameterType.FullName);
                    
                var obj = Activator.CreateInstance(type);

                if (obj != null)
                {
                    paramStr += obj.GetType().Name;
                }
            }

            TTLITradeWSDEV.ItradeWebServicesClient soap = new TTLITradeWSDEV.ItradeWebServicesClient();
            MethodInfo mth = soap.GetType().GetMethod("accountBalanceEnquiry");
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
                    requestHeaderType header = (requestHeaderType)obj;
                    paramStr += header.GetType();
                }
                if (typeof(BaseRequest_CType).IsAssignableFrom(obj.GetType()))
                {
                    BaseRequest_CType query = (BaseRequest_CType)obj;
                    paramStr += query.GetType();
                }
                if (typeof(BaseResponse_CType).IsAssignableFrom(obj.GetType()))
                {
                    BaseResponse_CType resp = (BaseResponse_CType)obj;
                    paramStr += resp.GetType();
                }
                i++;
            }
            //Assembly assembly = Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name);


            return Ok(BaseResponse.MakeResponse(new string[]
            {
                param.a,
                param.b,
                param.c + "",
                recvDate,
                "A",
                param.dict["aaaa"],
                paramStr
            }));
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
        [Route("ttl/submitSoapQuery")]
        public IHttpActionResult submitSoapQuery(TTLAPIRequest form)
        {
            try
            {
                var res = callSoapQuery(form);
                return Ok(BaseResponse.MakeResponse(res));
            }
            catch (Exception e)
            {
                return Ok(BaseResponse.MakeResponse("F001", e));
            }
        }







        public object callSoapQuery(TTLAPIRequest form)
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
                string json = new JavaScriptSerializer().Serialize(form.body);
                json = JsonConvert.SerializeObject(form.body);
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
                object resp = mth.Invoke(soap, p);

                if (respHeader.GetType().IsAssignableFrom(resp.GetType()))
                {
                    respHeader = (responseHeaderType)resp;
                }

                object res = p[2];
                return res;
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