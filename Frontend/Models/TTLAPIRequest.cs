using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            string[] keys = body.Keys.ToArray();
            for (int i = 0; i < keys.Count(); i++)
            {
                string key = keys[i];

                string pattern = @"[(?<key>.*?)]";
                string output = Regex.Replace(key, pattern, delegate (Match m) {
                    var str = m.Value;
                    if (str.Length >= 3)
                    {
                        str = str.Substring(1, str.Length - 1);
                        return str;
                    }
                    return "";
                });


            }

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