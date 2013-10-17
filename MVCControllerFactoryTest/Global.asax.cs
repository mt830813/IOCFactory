using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MVCUtil;
using IOCFactory;
using IOCFactoryModel;
using MVCControllerFactoryTest.Models;

namespace MVCControllerFactoryTest
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var factory = Factory.GetInst();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            InstRegist();

            ControllerBuilder.Current.SetControllerFactory(factory.Get<IControllerFactory>());
        }

        private void InstRegist()
        {
            var factory = Factory.GetInst();

            factory.Regist<IHelloWorld, HelloWorldModel>(InstType.DI);

            factory.Regist<IPlusWord, PlusWord>(InstType.Normal);

            factory.RegistFromFile(Server.MapPath("setting.xml"), IOCFactoryModel.Enum.FactoryMappingFilePattern.Unity);
        }
    }
}