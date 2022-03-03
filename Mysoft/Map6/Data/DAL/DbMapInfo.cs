using System.Reflection;

namespace Mysoft.Map6.Data.DAL
{
	internal class DbMapInfo
	{
		public string DbName { get; private set; }

		public string NetName { get; private set; }

		public PropertyInfo PropertyInfo { get; private set; }

		public DataColumnAttribute Attr { get; private set; }

		public DbMapInfo(string dbName, string netName, DataColumnAttribute attr, PropertyInfo prop)
		{
			DbName = dbName;
			NetName = netName;
			Attr = attr;
			PropertyInfo = prop;
		}
	}
}
