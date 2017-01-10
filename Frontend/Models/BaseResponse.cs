using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend.Models
{
    public class BaseResponse
    {
        public bool success;
        public string responseCode;
        public object data;
        public object errors;
        public string message;

        public static BaseResponse MakeResponse(object data, object errors = null)
        {
            BaseResponse res = new BaseResponse();
            res.success = true;
            res.data = data;
            res.errors = errors;
            res.responseCode = "A000";
            return res;
        }

        public static BaseResponse MakeResponse(string responseCode, object errors, object data = null, string message = null)
        {
            BaseResponse res = new BaseResponse();
            res.success = false;
            res.data = data;
            res.message = message;
            res.errors = errors;
            res.responseCode = responseCode;
            return res;
        }
        
    }
}