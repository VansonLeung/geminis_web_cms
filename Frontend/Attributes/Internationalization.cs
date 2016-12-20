using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Attributes
{
    public class InternationalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string locale = (string)filterContext.RouteData.Values["locale"] ?? "zh-HK";

            string language = "zh";
            string culture = "HK";

            try
            {
                language = locale.Split('-')[0];
                culture = locale.Split('-')[1];
            }
            catch (Exception e)
            {

            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", language, culture));
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", language, culture));
        }
    }
}