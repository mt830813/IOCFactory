using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "type", Namespace = "")]
    public class UnityType
    {
        public UnityType()
        {
            this.LifeTime = new LifeTime();
        }

        [XmlAttribute(AttributeName = "type")]
        public string TypeStr { get; set; }

        [XmlAttribute(AttributeName = "mapTo")]
        public string MapTo { get; set; }

        [XmlElement(ElementName = "lifetime")]
        public LifeTime LifeTime { get; set; }
    }
}
