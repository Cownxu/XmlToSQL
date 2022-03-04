using System;
using System.Runtime.Serialization;

namespace XmlToSQL.Mysoft.Exceptions
{
	[Serializable]
	public sealed class RollbackException : Exception
	{
		public RollbackException(string message)
			: base(message)
		{
		}

		private RollbackException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
