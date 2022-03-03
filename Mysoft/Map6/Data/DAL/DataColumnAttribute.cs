using System;

namespace Mysoft.Map6.Data.DAL
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class DataColumnAttribute : Attribute
	{
		public string Alias { get; set; }

		public bool PrimaryKey { get; set; }

		public bool TimeStamp { get; set; }

		public bool Identity { get; set; }

		public bool SeqGuid { get; set; }

		public bool IsNullable { get; set; }

		public string DefaultValue { get; set; }
	}
}
