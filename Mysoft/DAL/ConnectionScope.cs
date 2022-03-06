using System;
using System.Data.SqlClient;

namespace XmlToSQL.Mysoft.DAL
{
	public sealed class ConnectionScope : IDisposable
	{
        public static TransactionStackItem _item;

        //Data Source=.;Initial Catalog=UFDATA_010_2020;Integrated Security=True
		private static string s_connectionString = "";

        private static string s_providerName= "";//System.Data.SqlClient

        public ConnectionScope(TransactionStackItem item)
        {
            _item = item;
        }
        [ThreadStatic]
		private static ConnectionManager s_connection;

		internal static string ProviderName
		{
			get
			{
				if (s_connection == null)
				{
					return s_providerName;
				}
				ConnectionInfo topStackInfo = s_connection.GetTopStackInfo();
				return topStackInfo.ProviderName;
			}
		}

		internal static string ConnectionString
		{
			get
			{
				if (s_connection == null)
				{
					return s_connectionString;
				}
				ConnectionInfo topStackInfo = s_connection.GetTopStackInfo();
				return topStackInfo.ConnectionString;
			}
		}

		internal ConnectionManager Current => s_connection;

		internal event EventHandler<TransactionEventArgs> TransactionCommitting
		{
			add
			{
				TransactionInfo transactionInfo = GetTransactionInfo();
				transactionInfo.TransactionCommitting += value;
			}
			remove
			{
				TransactionInfo transactionInfo = GetTransactionInfo();
				transactionInfo.TransactionCommitting -= value;
			}
		}

		internal event EventHandler<TransactionEventArgs> TransactionCommitted
		{
			add
			{
				TransactionInfo transactionInfo = GetTransactionInfo();
				transactionInfo.TransactionCommitted += value;
			}
			remove
			{
				TransactionInfo transactionInfo = GetTransactionInfo();
				transactionInfo.TransactionCommitted -= value;
			}
		}

		internal static void SetDefaultConnection(string connectionString, string providerName)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			if (string.IsNullOrEmpty(providerName))
			{
				throw new ArgumentNullException("providerName");
			}
			s_connectionString = connectionString;
			s_providerName = providerName;
		}

		public ConnectionScope()
		{
			Init(TransactionMode.Inherits, null, null);
		}

		public ConnectionScope(TransactionMode mode)
		{
			Init(mode, null, null);
		}

		public ConnectionScope(TransactionMode mode, string connectionString, string providerName)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			if (string.IsNullOrEmpty(providerName))
			{
				throw new ArgumentNullException("providerName");
			}
			Init(mode, connectionString, providerName);
		}

		public ConnectionScope(TransactionMode mode, string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			Init(mode, connectionString, null);
		}

		private void Init(TransactionMode mode, string connectionString, string providerName)
		{
			string text =s_connectionString;
			string text2 =providerName;
			if (s_connection == null)
			{
				s_connection = new ConnectionManager(_item);
                text = _item.Info.ConnectionString;
                text2 = _item.Info.ProviderName;
                mode = _item.Mode;
            }
			else
            {
                s_connection = new ConnectionManager(_item);
				ConnectionInfo topStackInfo = s_connection.GetTopStackInfo();
				if (topStackInfo != null)
				{
					if (string.IsNullOrEmpty(connectionString))
					{
						text = topStackInfo.ConnectionString;
					}
					if (string.IsNullOrEmpty(providerName))
					{
						text2 = topStackInfo.ProviderName;
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = s_connectionString;
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = s_providerName;
			}
			s_connection.PushTransactionMode(mode, text, text2);
		}

		public SqlBulkCopy CreateSqlBulkCopy(SqlBulkCopyOptions copyOptions)
		{
			return s_connection.CreateSqlBulkCopy(copyOptions);
		}

		public void Dispose()
		{
			if (s_connection != null && !s_connection.PopTransactionMode())
			{
				ForceClose();
			}
		}

		public void Commit()
		{
			if (s_connection == null)
			{
				throw new InvalidOperationException("还没有打开数据库连接，无法完成提交请求。");
			}
			s_connection.Commit();
		}

		public void Rollback(string message)
		{
			if (s_connection == null)
			{
				throw new InvalidOperationException("还没有打开数据库连接，无法完成回滚请求。");
			}
			s_connection.Rollback(message);
		}

		internal static void ForceClose()
		{
			if (s_connection == null)
			{
				return;
			}
			try
			{
				s_connection.Dispose();
			}
			catch
			{
			}
			finally
			{
				s_connection = null;
			}
		}

		internal TransactionInfo GetTransactionInfo()
		{
			ConnectionInfo topStackInfo = Current.GetTopStackInfo();
			if (topStackInfo == null || topStackInfo.TransactionInfo == null)
			{
				throw new InvalidOperationException("当前的作用域不支持事务操作。");
			}
			return topStackInfo.TransactionInfo;
		}
	}
}
