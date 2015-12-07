using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebStorage.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("", "Shared/{rootSharingId}/{contentId}", new
            {
                controller = "SharedFile",
                action = "Index",
                contentId = UrlParameter.Optional
            });

            routes.MapRoute("", "Shared/{rootSharingId}/{action}/{contentId}", new
            {
                controller = "SharedFile",
                action = "Index",
                contentId = UrlParameter.Optional
            });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "File", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
