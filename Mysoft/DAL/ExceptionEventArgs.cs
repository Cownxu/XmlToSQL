using System;
using System.Collections;
using System.Data.Common;

namespace XmlToSQL.Mysoft.DAL
{
	public class ExceptionEventArgs : EventArgs
	{
		public DbCommand Command { get; internal set; }

		public Exception Exception { get; internal set; }

		public Hashtable UserData { get; internal set; }
	}
}
