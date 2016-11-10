using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TheMovieDB
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //Register MovieLib page
            routes.MapRoute("MovieLib", "MovieLib/{action}/{id}", new { controller = "MovieLib", action = "Index", id = UrlParameter.Optional });

            //Register Login Page
            routes.MapRoute("Login", "Login/{action}/{id}", new { controller = "Login", action = "Index", id = UrlParameter.Optional });

            //Register Login Page
            routes.MapRoute("Editor", "Editor/{action}/{id}", new { controller = "Editor", action = "Index", id = UrlParameter.Optional });

        }
    }
}