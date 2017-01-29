using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    /*
    {
      "session": "63954EBF29264343C5524C7DC16D7B1C.geminis",
      "header": {
        "dateDT": "20170129135438"
      },
      "error": {
        "errCode": "200"
      }
    }
    */
    public class QPIAPIResponse
    {
        public string session { get; set; };
        public Header header { get; set; };
        public Error error { get; set; }
    }

    public class Header
    {
        public string dateDT { get; set; }
    }

    public class Error
    {
        public string errCode { get; set; }
    }
}