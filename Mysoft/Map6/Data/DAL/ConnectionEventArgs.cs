using System;
using System.Data.Common;

namespace Mysoft.Map6.Data.DAL
{
	public class ConnectionEventArgs : EventArgs
	{
		public DbConnection Connection { get; internal set; }
	}
}
