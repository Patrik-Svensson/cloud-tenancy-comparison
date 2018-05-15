using WebApplication.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity.Infrastructure.Interception;

namespace WebApplication
{
	public class WebApiApplication : System.Web.HttpApplication
	{
        public static int[] dummy1 = new int[1000 * 1000 * 10];
        protected void Application_Start()
		{
            for (int i = 0; i < dummy1.Length; i++)
                dummy1[i] = i;

            AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			DbInterception.Add(new SchoolInterceptorLogging());
		}
	}
}
