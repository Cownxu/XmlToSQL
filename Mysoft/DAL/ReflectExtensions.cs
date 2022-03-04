using System;
using System.Data;
using ClownFish.Base.Reflection;

namespace XmlToSQL.Mysoft.DAL
{
	internal static class ReflectExtensions
	{
		internal static void ReflectSetValue<T>(this T obj, DbMapInfo info, object val, string colName) where T : class
		{
			try
			{
				if (val != null && !DBNull.Value.Equals(val))
				{
					if (info.Attr != null && info.Attr.TimeStamp)
					{
						info.PropertyInfo.FastSetValue(obj, val.ConvertToTimeStamp(info.PropertyInfo.PropertyType));
					}
					else
					{
						info.PropertyInfo.FastSetValue(obj, val.ConvertToType(info.PropertyInfo.PropertyType));
					}
				}
			}
			catch (Exception innerException)
			{
				throw new DataException("Data convert failed, current column name: " + colName, innerException);
			}
		}
	}
}
