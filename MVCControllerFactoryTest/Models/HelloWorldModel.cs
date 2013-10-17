using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCControllerFactoryTest.Models
{
    public class HelloWorldModel : IHelloWorld
    {
        IPlusWord obj;
        public HelloWorldModel(IPlusWord obj)
        {
            this.obj = obj;
        }

        public string HelloWorld()
        {
            return "Hello World " + this.obj.GetPlusWord();
        }
    }
}