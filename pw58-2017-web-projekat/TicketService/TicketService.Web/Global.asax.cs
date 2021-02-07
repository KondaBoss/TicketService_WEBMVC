using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TicketService.Data.Models;
using TicketService.Data.Services;

namespace TicketService.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            List<Customer> customers = CustomerData.GetCustomers();
            HttpContext.Current.Application["customers"] = customers;

            List<Administrator> administrators = AdministratorData.GetAdministrators();
            HttpContext.Current.Application["administrators"] = administrators;

            List<Salesman> salesmen = SalesmanData.GetSalesmen();
            HttpContext.Current.Application["salesmen"] = salesmen;
        }
    }
}
