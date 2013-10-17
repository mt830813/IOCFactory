using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOCFactoryModel.Interface;
using IOCFactoryModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;


namespace IOCFactory.Model.Imp.RegistContextReader
{
    public class NormalReader : IRegistContextReader
    {
        public RegistMappingContextCollection GetMappingContexts(string fileUrl)
        {
            RegistMappingContextCollection returnValue = new RegistMappingContextCollection();
            try
            {
                var serializer = new JavaScriptSerializer();
                //serializer.WriteObject(fs, returnValue);

                returnValue = serializer.Deserialize<RegistMappingContextCollection>(File.ReadAllText(fileUrl));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }
    }
}
