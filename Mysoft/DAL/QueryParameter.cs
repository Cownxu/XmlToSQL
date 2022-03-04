using System;

namespace XmlToSQL.Mysoft.DAL
{
	public sealed class QueryParameter
	{
		private object _val;

		public object Value => _val;

		public QueryParameter(object val)
		{
			_val = val;
		}

		public static explicit operator QueryParameter(string value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(DBNull value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(bool value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(byte value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(int value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(long value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(short value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(float value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(double value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(decimal value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(Guid value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(DateTime value)
		{
			return new QueryParameter(value);
		}

		public static implicit operator QueryParameter(byte[] value)
		{
			return new QueryParameter(value);
		}
	}
}
