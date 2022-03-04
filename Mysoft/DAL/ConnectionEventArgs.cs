using System;
using System.Data.Common;

namespace XmlToSQL.Mysoft.DAL
{
	public class ConnectionEventArgs : EventArgs
	{
		public DbConnection Connection { get; internal set; }
	}
}
