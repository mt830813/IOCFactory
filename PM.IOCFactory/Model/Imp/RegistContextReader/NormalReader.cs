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
                using (Stream sr = File.Open(fileUrl, FileMode.Open))
                {
                    returnValue = this.GetMappingContexts(sr);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }


        public RegistMappingContextCollection GetMappingContexts(Stream stream)
        {
            RegistMappingContextCollection returnValue = new RegistMappingContextCollection();

            if (stream.CanRead)
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    var serializer = new JavaScriptSerializer();
                    returnValue = serializer.Deserialize<RegistMappingContextCollection>(sr.ReadToEnd());
                }
            }
            else
            {
                throw new Exception("input stream can't read");
            }
            return returnValue;
        }
    }
}
