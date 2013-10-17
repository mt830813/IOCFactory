using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "container", Namespace = "")]
    public class Container
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "types")]
        public UnityTypes Types { get; set; }
    }
}
