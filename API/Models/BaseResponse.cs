using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class BaseResponse
    {
        public string responseCode;
        public object data;
        public object errors;

        public static BaseResponse MakeResponse(object data, object errors = null)
        {
            BaseResponse res = new BaseResponse();
            res.data = data;
            res.errors = errors;
            res.responseCode = "A000";
            return res;
        }

        public static BaseResponse MakeResponse(string responseCode, object errors, object data = null)
        {
            BaseResponse res = new BaseResponse();
            res.data = data;
            res.errors = errors;
            res.responseCode = responseCode;
            return res;
        }
        
    }
}