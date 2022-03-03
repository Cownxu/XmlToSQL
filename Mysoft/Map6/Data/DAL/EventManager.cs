using System;
using System.Collections;
using System.Data.Common;

namespace Mysoft.Map6.Data.DAL
{
	public static class EventManager
	{
		public static event EventHandler<ConnectionEventArgs> ConnectionOpened;

		public static event EventHandler<CommandEventArgs> BeforeExecute;

		public static event EventHandler<CommandEventArgs> AfterExecute;

		public static event EventHandler<ExceptionEventArgs> OnException;

		internal static event EventHandler<TransactionEventArgs> BeforeTransactionCommit;

		internal static event EventHandler<TransactionEventArgs> AfterTransactionCommit;

		internal static void FireBeforeTransactionCommit(ConnectionInfo conn)
		{
			EventHandler<TransactionEventArgs> beforeTransactionCommit = EventManager.BeforeTransactionCommit;
			if (beforeTransactionCommit != null)
			{
				TransactionEventArgs transactionEventArgs = new TransactionEventArgs();
				transactionEventArgs.ConnectionInfo = conn;
				transactionEventArgs.BeginTime = conn.TransactionBeginTime;
				beforeTransactionCommit(null, transactionEventArgs);
			}
		}

		internal static void FireAfterTransactionCommit(ConnectionInfo conn)
		{
			EventHandler<TransactionEventArgs> afterTransactionCommit = EventManager.AfterTransactionCommit;
			if (afterTransactionCommit != null)
			{
				TransactionEventArgs transactionEventArgs = new TransactionEventArgs();
				transactionEventArgs.BeginTime = conn.TransactionBeginTime;
				afterTransactionCommit(null, transactionEventArgs);
			}
		}

		internal static void FireConnectionOpened(DbConnection conn)
		{
			EventHandler<ConnectionEventArgs> connectionOpened = EventManager.ConnectionOpened;
			if (connectionOpened != null)
			{
				ConnectionEventArgs connectionEventArgs = new ConnectionEventArgs();
				connectionEventArgs.Connection = conn;
				connectionOpened(null, connectionEventArgs);
			}
		}

		internal static Hashtable FireBeforeExecute(DbCommand cmd)
		{
			Hashtable result = null;
			EventHandler<CommandEventArgs> beforeExecute = EventManager.BeforeExecute;
			if (beforeExecute != null)
			{
				CommandEventArgs commandEventArgs = new CommandEventArgs();
				commandEventArgs.Command = cmd;
				beforeExecute(null, commandEventArgs);
				result = commandEventArgs.InternalGetUserData();
			}
			return result;
		}

		internal static void FireAfterExecute(DbCommand cmd, Hashtable data)
		{
			EventHandler<CommandEventArgs> afterExecute = EventManager.AfterExecute;
			if (afterExecute != null)
			{
				CommandEventArgs commandEventArgs = new CommandEventArgs();
				commandEventArgs.Command = cmd;
				commandEventArgs.UserData = data;
				afterExecute(null, commandEventArgs);
			}
		}

		internal static void FireOnException(DbCommand cmd, Exception ex, Hashtable data)
		{
			EventHandler<ExceptionEventArgs> onException = EventManager.OnException;
			if (onException != null)
			{
				ExceptionEventArgs exceptionEventArgs = new ExceptionEventArgs();
				exceptionEventArgs.Command = cmd;
				exceptionEventArgs.Exception = ex;
				exceptionEventArgs.UserData = data;
				try
				{
					onException(null, exceptionEventArgs);
				}
				catch
				{
				}
			}
		}
	}
}
