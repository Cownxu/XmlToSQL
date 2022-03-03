using System;
using System.Data.Common;

namespace Mysoft.Map6.Data.DAL
{
	internal class ConnectionInfo
	{
		public string ConnectionString { get; set; }

		public string ProviderName { get; set; }

		public DbConnection Connection { get; set; }

		public TransactionInfo TransactionInfo { get; set; }

		public DateTime TransactionBeginTime { get; set; }

		public ConnectionInfo(string connectionString, string providerName)
		{
			ConnectionString = connectionString;
			ProviderName = providerName;
		}

		public bool IsSame(ConnectionInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			return ConnectionString == info.ConnectionString && ProviderName == info.ProviderName;
		}
	}
}
