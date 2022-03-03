using System;
using System.Data.Common;

namespace Mysoft.Map6.Data.DAL
{
	public class MockEventArgs : EventArgs
	{
		public DbCommand Command { get; internal set; }

		public object MockResult { get; set; }
	}
}
