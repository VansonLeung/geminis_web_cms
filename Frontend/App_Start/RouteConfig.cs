using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Frontend
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                    name: "Page",
                    url: "Page/{category}/{id}",
                    defaults: new
                    {
                        locale = "zh-HK",
                        controller = "Page",
                        action = "Index",
                        category = UrlParameter.Optional,
                        id = UrlParameter.Optional
                    }
                );

            routes.MapRoute(
                    name: "PageLocale",
                    url: "{locale}/Page/{category}/{id}",
                    defaults: new
                    {
                        locale = "zh-HK",
                        controller = "Page",
                        action = "Index",
                        category = UrlParameter.Optional,
                        id = UrlParameter.Optional
                    }
                );

            routes.MapRoute(
                name: "Default",
                url: "{locale}/{controller}/{action}/{id}",
                defaults: new
                {
                    locale = "zh-HK",
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "APISession",
                url: "api/session/{action}",
                defaults: new
                {
                    controller = "Session",
                    action = "Index"
                }
            );
            routes.MapRoute(
                name: "APIIPAddress",
                url: "api/ipaddress/{action}",
                defaults: new
                {
                    controller = "IPAddress",
                    action = "Index"
                }
            );
            routes.MapRoute(
                name: "APIUserCode",
                url: "api/usercode/{action}",
                defaults: new
                {
                    controller = "UserCode",
                    action = "Index"
                }
            );
            routes.MapRoute(
                name: "APIUser",
                url: "api/user/{action}",
                defaults: new
                {
                    controller = "User",
                    action = "Index"
                }
            );
        }
    }
}
