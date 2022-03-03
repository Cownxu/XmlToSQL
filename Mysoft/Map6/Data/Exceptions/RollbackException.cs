using System;
using System.Runtime.Serialization;

namespace Mysoft.Map6.Data.Exceptions
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
