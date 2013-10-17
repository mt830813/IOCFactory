using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCControllerFactoryTest.Models
{
    public class PlusWord : IPlusWord
    {
        public string GetPlusWord()
        {
            return "Plus 1";
        }
    }
}