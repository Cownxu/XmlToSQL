using System;
using System.Collections;

namespace XmlToSQL.Mysoft.DAL
{
	internal class TransactionEventArgs : EventArgs
	{
		internal ConnectionInfo ConnectionInfo { get; set; }

		public DateTime BeginTime { get; internal set; }

		public Hashtable UserData { get; internal set; }
	}
}
