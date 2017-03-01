using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend.Models
{
    public class TTLAPIRequest
    {
        public class TTLAPIRequestForm
        {
            public TTLAPIRequest form { get; set; }
        }

        public TTLAPIRequest()
        {

        }

        public TTLAPIRequest(
            string name,
            Dictionary<string, object> body,
            string otp = ""
        )
        {
            this.credentials = new TTLAPIRequestCredentials();
            this.credentials.username = "";
            this.credentials.password = "";

            this.header = new TTLAPIRequestHeader();
            this.header.version = "1";
            this.header.traceNo = "1";

            this.name = name;
            this.body = body;

            this.otp = otp;
    }

        public string name { get; set; } = "";
        public string otp { get; set; } = "";
        public TTLAPIRequestCredentials credentials { get; set; }
        public TTLAPIRequestHeader header { get; set; }
        public Dictionary<string,object> body { get; set; }
    }

    public class TTLAPIRequestCredentials
    {
        public string username { get; set; } = "";
        public string password { get; set; } = "";
    }

    public class TTLAPIRequestHeader
    {
        public string version { get; set; } = "";
        public string traceNo { get; set; } = "";
    }
}