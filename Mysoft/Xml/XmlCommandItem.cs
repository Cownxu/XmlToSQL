using CownxuFish.Base.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Xml.Serialization;

namespace XmlToSQL.Mysoft.Xml
{
    [Serializable]
    [XmlType("XmlCommand")]
    public class XmlCommandItem
    {
        [XmlAttribute("Name")]
        public string CommandName;

        [XmlArrayItem("Parameter")]
        public List<XmlCmdParameter> Parameters = new List<XmlCmdParameter>();

        [XmlElement]
        public XmlCdata CommandText;

        [DefaultValue(CommandType.Text)]
        [XmlAttribute]
        public CommandType CommandType = CommandType.Text;

        [DefaultValue(30)]
        [XmlAttribute]
        public int Timeout = 30;
    }
}
