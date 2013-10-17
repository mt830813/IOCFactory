using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "types", Namespace = "")]
    public class UnityTypes
    {
        [XmlElement(ElementName = "type")]
        public UnityType[] Objects { get; set; }
    }
}
