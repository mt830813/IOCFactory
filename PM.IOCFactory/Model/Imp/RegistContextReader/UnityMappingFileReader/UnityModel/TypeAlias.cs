using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "typeAlias", Namespace = "")]
    public class TypeAlias
    {
        [XmlAttribute(AttributeName = "alias")]
        public string Alias { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string TypeStr { get; set; }
    }
}
