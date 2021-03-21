using QFinans.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QFinans
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SqlDependency.Start(ConfigurationManager.ConnectionStrings["NotificationConnection"].ConnectionString);
        }

        //Sunucuda çalışmıyor
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    if (HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false))
        //    {
        //        Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
        //    }
        //}

        protected void Application_End()
        {
            SqlDependency.Stop(ConfigurationManager.ConnectionStrings["NotificationConnection"].ConnectionString);
        }
    }
}
