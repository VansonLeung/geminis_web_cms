using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using WebApplication2.Resources;

namespace WebApplication2.Helpers
{
    public class ResHelper
    {
        public static string S(string key)
        {
            return new ResourceManager(typeof(Resource)).GetString(key);
        }
    }
}