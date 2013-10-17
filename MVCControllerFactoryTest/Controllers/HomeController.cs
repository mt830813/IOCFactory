using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCControllerFactoryTest.Models;

namespace MVCControllerFactoryTest.Controllers
{
    public class HomeController : Controller
    {
        private IHelloWorld model;

        public HomeController(IHelloWorld model)
        {
            this.model = model;
        }

        public ActionResult Index()
        {
            return View("HelloWorld", model);
        }
    }
}