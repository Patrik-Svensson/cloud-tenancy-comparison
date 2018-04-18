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
using Autofac;
using Autofac.Integration.WebApi;
using Autofac.Integration.Mvc;
using System.Reflection;

namespace WebApplication
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
        {
            // AutoFac Dependency Injection
            var builder = new ContainerBuilder();
            RegisterServices(builder);


            // Register your Web API controllers.
            Assembly currentProject = typeof(WebApiApplication).Assembly;
            builder.RegisterApiControllers(currentProject);
            builder.RegisterControllers(currentProject);


            var container = builder.Build();

            // Set the dependency resolver to be Autofac.
            // WebApi
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            // MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            //
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DbInterception.Add(new SchoolInterceptorLogging());
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            // TODO: Review settings, determine "lifetime" of objects
            // Per fråga: 
            //   builder.RegisterType<Type>.As<IInterface>.InstancePerRequest()
            // Global / statisk
            //   builder.RegisterInstance<IInterface>(new Type());
            // Ny för varje ställa den används
            //   builder.RegisterType<Type>.As<IInterface>()


            // Setup implementations
            // If query id provider can be shared use
            // builder.RegisterInstance<ITenantIdProvider>(new QueryIdProvider());
            builder.RegisterType<QueryIdProvider>().As<ITenantIdProvider>().InstancePerRequest();
            builder.RegisterType<TenantCache>().As<ICache>();
            builder.RegisterType<SchoolContext>().InstancePerRequest();
            
        }
    }
}
