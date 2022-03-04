using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace XmlToSQL.Mysoft.Exceptions
{
	[Serializable]
	public sealed class DbExceuteException : Exception
	{
		public DbCommand Command { get; private set; }

		public DbExceuteException(Exception innerException, DbCommand command)
			: base(innerException.Message, innerException)
		{
			if (innerException == null)
			{
				throw new ArgumentNullException("innerException");
			}
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			Command = command;
		}

		private DbExceuteException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
