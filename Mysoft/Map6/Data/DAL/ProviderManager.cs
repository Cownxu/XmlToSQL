using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Mysoft.Map6.Data.DAL
{
	public static class ProviderManager
	{
		private static readonly DbProviderFactory s_sqlserverFactory = SqlClientFactory.Instance;

		private static ConcurrentDictionary<string, DbProviderFactory> s_providerDict = new ConcurrentDictionary<string, DbProviderFactory>();

		public static DbProviderFactory ProviderFactory
		{
			get
			{
				string providerName = ConnectionScope.ProviderName;
				if (string.IsNullOrEmpty(providerName))
				{
					return s_sqlserverFactory;
				}
				if (string.Equals(providerName, "System.Data.SqlClient", StringComparison.OrdinalIgnoreCase))
				{
					return s_sqlserverFactory;
				}
				if (s_providerDict.TryGetValue(providerName, out var value))
				{
					return value;
				}
				value = DbProviderFactories.GetFactory(providerName);
				s_providerDict.TryAdd(providerName, value);
				return value;
			}
		}

		public static DbConnection CreateConnection()
		{
			return ProviderFactory.CreateConnection();
		}

		internal static DbConnection CreateConnection(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			DbConnection dbConnection = ProviderFactory.CreateConnection();
			dbConnection.ConnectionString = connectionString;
			return dbConnection;
		}

		public static DbCommand CreateCommand()
		{
			return ProviderFactory.CreateCommand();
		}

		public static DbDataAdapter CreateDataAdapter()
		{
			return ProviderFactory.CreateDataAdapter();
		}

		public static DbParameter CreateParameter()
		{
			return ProviderFactory.CreateParameter();
		}

		public static DbParameter CreateParameter(string name, object value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			DbParameter dbParameter = ProviderFactory.CreateParameter();
			dbParameter.ParameterName = name;
			dbParameter.Value = value;
			return dbParameter;
		}

		internal static DbParameter CreateParameter(string name, DbType type, object value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			DbParameter dbParameter = ProviderFactory.CreateParameter();
			dbParameter.ParameterName = name;
			dbParameter.Value = value;
			dbParameter.DbType = type;
			return dbParameter;
		}
	}
}
