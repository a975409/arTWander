using arTWander.Models;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace arTWander
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			//HttpRuntimeSection section = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
			//int maxFileSize = section.MaxRequestLength * 1024;
			//if (Request.ContentLength > maxFileSize)
			//{
			//	try
			//	{
			//		Response.Redirect("~/SizeError.aspx");
			//	}
			//	catch (Exception ex)
			//	{
			//		Logger logger = LogManager.GetCurrentClassLogger();
			//		logger.Warn(ex.Message);
			//	}
			//}
		}
	}
}
