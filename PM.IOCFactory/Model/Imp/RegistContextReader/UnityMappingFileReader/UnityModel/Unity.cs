using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "unity", Namespace = "")]
    public class Unity
    {
        [XmlElement(ElementName = "container")]
        public Container[] Containers { get; set; }
    }
}
