using System;
using System.Collections;
using System.Data.Common;

namespace XmlToSQL.Mysoft.DAL
{
	public class TransactionInfo : IDisposable
	{
		internal DbTransaction Transaction { get; }

		internal Hashtable UserData { get; set; }

		internal bool HasRegistTransactionCommitted
		{
			get
			{
				EventHandler<TransactionEventArgs> transactionCommitted = this.TransactionCommitted;
				return transactionCommitted != null;
			}
		}

		internal bool HasTransactionCommitted { get; set; }

		internal event EventHandler<TransactionEventArgs> TransactionCommitting;

		internal event EventHandler<TransactionEventArgs> TransactionCommitted;

		internal TransactionInfo(DbTransaction transaction)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			Transaction = transaction;
		}

		internal void OnTransactionCommitting(ConnectionInfo connectionInfo)
		{
			EventHandler<TransactionEventArgs> transactionCommitting = this.TransactionCommitting;
			if (transactionCommitting != null)
			{
				TransactionEventArgs transactionEventArgs = new TransactionEventArgs();
				transactionEventArgs.BeginTime = connectionInfo.TransactionBeginTime;
				transactionEventArgs.ConnectionInfo = connectionInfo;
				transactionEventArgs.UserData = UserData;
				transactionCommitting(null, transactionEventArgs);
			}
		}

		internal void OnTransactionCommitted(ConnectionInfo connectionInfo)
		{
			EventHandler<TransactionEventArgs> transactionCommitted = this.TransactionCommitted;
			if (transactionCommitted != null)
			{
				TransactionEventArgs transactionEventArgs = new TransactionEventArgs();
				transactionEventArgs.BeginTime = connectionInfo.TransactionBeginTime;
				transactionEventArgs.ConnectionInfo = connectionInfo;
				transactionEventArgs.UserData = UserData;
				transactionCommitted(null, transactionEventArgs);
			}
		}

		public void Dispose()
		{
			Transaction?.Dispose();
		}
	}
}
