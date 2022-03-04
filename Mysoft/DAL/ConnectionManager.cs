using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using XmlToSQL.Mysoft.Exceptions;
using Mysoft.Map6.Data.Test;

namespace XmlToSQL.Mysoft.DAL
{
	internal class ConnectionManager : IDisposable
	{
		private readonly Stack<TransactionStackItem> _transactionModes = new Stack<TransactionStackItem>();

		internal ConnectionInfo OpenTopStackInfo(bool hit = false)
		{
			TransactionStackItem transactionStackItem = _transactionModes.Peek();
			ConnectionInfo info = transactionStackItem.Info;
			if (info.Connection == null)
			{
				info.Connection = ProviderManager.CreateConnection(info.ConnectionString);
			}
			ConnectionState state = info.Connection.State;
			if (state == ConnectionState.Closed || state == ConnectionState.Broken)
			{
				info.Connection.Open();
				EventManager.FireConnectionOpened(info.Connection);
			}
			if (transactionStackItem.EnableTranscation && info.TransactionInfo == null)
			{
				info.TransactionInfo = new TransactionInfo(info.Connection.BeginTransaction());
				info.TransactionBeginTime = DateTime.Now;
			}
			if (hit)
			{
				foreach (TransactionStackItem transactionMode in _transactionModes)
				{
					if (transactionMode.Info == info)
					{
						transactionMode.HitCount++;
					}
				}
			}
			return info;
		}

		public T ExecuteCommand<T>(DbCommand command, Func<DbCommand, T> func)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			object result = MockResult.GetResult();
			if (result != null)
			{
				return (T)result;
			}
			ConnectionInfo connectionInfo = OpenTopStackInfo();
			command.Connection = connectionInfo.Connection;
			if (connectionInfo.TransactionInfo != null)
			{
				command.Transaction = connectionInfo.TransactionInfo.Transaction;
			}
			Hashtable data = EventManager.FireBeforeExecute(command);
			try
			{
				T result2 = func(command);
				EventManager.FireAfterExecute(command, data);
				foreach (TransactionStackItem transactionMode in _transactionModes)
				{
					if (transactionMode.Info == connectionInfo)
					{
						transactionMode.HitCount++;
					}
				}
				return result2;
			}
			catch (Exception ex)
			{
				EventManager.FireOnException(command, ex, data);
				throw new DbExceuteException(ex, command);
			}
			finally
			{
				command.Connection = null;
				command.Transaction = null;
			}
		}

		public SqlBulkCopy CreateSqlBulkCopy(SqlBulkCopyOptions copyOptions)
		{
			ConnectionInfo connectionInfo = OpenTopStackInfo(hit: true);
			SqlConnection sqlConnection = connectionInfo.Connection as SqlConnection;
			if (sqlConnection == null)
			{
				throw new InvalidOperationException("只支持在SqlServer环境下使用SqlBulkCopy。");
			}
			SqlTransaction externalTransaction = connectionInfo.TransactionInfo.Transaction as SqlTransaction;
			return new SqlBulkCopy(sqlConnection, copyOptions, externalTransaction);
		}

		internal void PushTransactionMode(TransactionMode mode, string connectionString, string providerName)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			if (string.IsNullOrEmpty(providerName))
			{
				throw new ArgumentNullException("providerName");
			}
			TransactionStackItem transactionStackItem = new TransactionStackItem();
			transactionStackItem.Mode = mode;
			foreach (TransactionStackItem transactionMode in _transactionModes)
			{
				if (transactionMode.Info.ConnectionString == connectionString)//&& transactionMode.Info.ProviderName == providerName
				{
					transactionStackItem.Info = transactionMode.Info;
					transactionStackItem.EnableTranscation = transactionMode.EnableTranscation;
					transactionStackItem.CanClose = false;
					break;
				}
			}
			switch (mode)
			{
			case TransactionMode.Required:
				if (transactionStackItem.Info == null || (transactionStackItem.Info.TransactionInfo != null && transactionStackItem.Info.TransactionInfo.HasTransactionCommitted))
				{
					transactionStackItem.Info = new ConnectionInfo(connectionString, providerName);
					transactionStackItem.CanClose = true;
				}
				transactionStackItem.EnableTranscation = true;
				break;
			case TransactionMode.RequiresNew:
				transactionStackItem.Info = new ConnectionInfo(connectionString, providerName);
				transactionStackItem.EnableTranscation = true;
				transactionStackItem.CanClose = true;
				break;
			case TransactionMode.Suppress:
				transactionStackItem.Info = new ConnectionInfo(connectionString, providerName);
				transactionStackItem.EnableTranscation = false;
				transactionStackItem.CanClose = true;
				break;
			default:
				if (transactionStackItem.Info == null)
				{
					ConnectionInfo connectionInfo2 = (transactionStackItem.Info = new ConnectionInfo(connectionString, providerName));
					transactionStackItem.CanClose = true;
				}
				break;
			}
			_transactionModes.Push(transactionStackItem);
		}

		internal bool PopTransactionMode()
		{
			if (_transactionModes.Count == 0)
			{
				return false;
			}
			TransactionStackItem transactionStackItem = _transactionModes.Pop();
			if (transactionStackItem.Mode == TransactionMode.Required)
			{
				bool flag = false;
				foreach (TransactionStackItem transactionMode in _transactionModes)
				{
					if (transactionMode.Info.IsSame(transactionStackItem.Info) && (transactionMode.Mode == TransactionMode.Required || transactionMode.Mode == TransactionMode.RequiresNew))
					{
						flag = true;
						break;
					}
				}
				if (!flag && transactionStackItem.Info.TransactionInfo != null)
				{
					IDisposable transaction = transactionStackItem.Info.TransactionInfo.Transaction;
					transaction.Dispose();
					transactionStackItem.Info.TransactionInfo = null;
				}
			}
			else if (transactionStackItem.Mode == TransactionMode.RequiresNew && transactionStackItem.Info.TransactionInfo != null)
			{
				IDisposable transaction2 = transactionStackItem.Info.TransactionInfo.Transaction;
				transaction2.Dispose();
				transactionStackItem.Info.TransactionInfo = null;
			}
			if (transactionStackItem.CanClose && transactionStackItem.Info.Connection != null)
			{
				IDisposable connection = transactionStackItem.Info.Connection;
				connection.Dispose();
				transactionStackItem.Info.Connection = null;
			}
			return _transactionModes.Count != 0;
		}

		public void Commit()
		{
			TransactionStackItem transactionStackItem = _transactionModes.Peek();
			if (transactionStackItem.HitCount == 0)
			{
				return;
			}
			if (transactionStackItem.Mode != TransactionMode.Required && transactionStackItem.Mode != TransactionMode.RequiresNew)
			{
				throw new InvalidOperationException("未在构造函数中指定TransactionMode.Required参数,不能调用Commit方法。");
			}
			if (transactionStackItem.Info.TransactionInfo == null)
			{
				throw new InvalidOperationException("当前的作用域不支持事务操作。");
			}
			transactionStackItem = _transactionModes.Pop();
			bool flag = false;
			try
			{
				if (transactionStackItem.Mode == TransactionMode.RequiresNew)
				{
					flag = false;
				}
				else if (ParentEnableTransaction(transactionStackItem))
				{
					flag = true;
				}
			}
			finally
			{
				_transactionModes.Push(transactionStackItem);
			}
			if (!flag)
			{
				transactionStackItem.Info.TransactionInfo.OnTransactionCommitting(transactionStackItem.Info);
				EventManager.FireBeforeTransactionCommit(transactionStackItem.Info);
				transactionStackItem.Info.TransactionInfo.Transaction.Commit();
				transactionStackItem.Info.TransactionInfo.HasTransactionCommitted = true;
				transactionStackItem.Info.TransactionInfo.OnTransactionCommitted(transactionStackItem.Info);
				EventManager.FireAfterTransactionCommit(transactionStackItem.Info);
			}
		}

		private bool ParentEnableTransaction(TransactionStackItem current)
		{
			TransactionStackItem[] array = _transactionModes.ToArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				TransactionStackItem transactionStackItem = array[i];
				if (transactionStackItem.Info.IsSame(current.Info))
				{
					if (transactionStackItem.Mode == TransactionMode.Suppress || (transactionStackItem.Info.TransactionInfo != null && transactionStackItem.Info.TransactionInfo.HasTransactionCommitted))
					{
						return false;
					}
					if (transactionStackItem.Mode == TransactionMode.Required || transactionStackItem.Mode == TransactionMode.RequiresNew)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Rollback(string message)
		{
			TransactionStackItem transactionStackItem = _transactionModes.Peek();
			if (transactionStackItem.Info.TransactionInfo == null)
			{
				throw new InvalidOperationException("当前的作用域不支持事务操作。*");
			}
			throw new RollbackException(message);
		}

		public ConnectionInfo GetTopStackInfo()
		{
			if (_transactionModes.Count > 0)
			{
				return _transactionModes.Peek().Info;
			}
			return null;
		}

		public void Dispose()
		{
			foreach (TransactionStackItem transactionMode in _transactionModes)
			{
				if (transactionMode.Info.TransactionInfo != null)
				{
					transactionMode.Info.TransactionInfo.Dispose();
					transactionMode.Info.TransactionInfo = null;
				}
				if (transactionMode.Info.Connection != null)
				{
					transactionMode.Info.Connection.Dispose();
					transactionMode.Info.Connection = null;
				}
			}
		}
	}
}
