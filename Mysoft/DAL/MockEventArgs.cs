using System;
using System.Data.Common;

namespace XmlToSQL.Mysoft.DAL
{
	public class MockEventArgs : EventArgs
	{
		public DbCommand Command { get; internal set; }

		public object MockResult { get; set; }
	}
}
