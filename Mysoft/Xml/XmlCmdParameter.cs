using System;
using System.ComponentModel;
using System.Data;
using System.Xml.Serialization;

namespace XmlToSQL.Mysoft.Xml
{
	[Serializable]
	public class XmlCmdParameter
	{
		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public DbType Type;

		[DefaultValue(0)]
		[XmlAttribute]
		public int Size;

		[DefaultValue(ParameterDirection.Input)]
		[XmlAttribute]
		public ParameterDirection Direction = ParameterDirection.Input;
	}
}
