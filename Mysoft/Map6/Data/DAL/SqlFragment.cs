using System;

namespace Mysoft.Map6.Data.DAL
{
	public sealed class SqlFragment
	{
		public string Value { get; private set; }

		public SqlFragment(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			Value = text;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
