using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ClownFish.Base.Reflection;

namespace Mysoft.Map6.Data.DAL
{
	internal static class TypeDescriptionCache
	{
		private static readonly Hashtable s_typeInfoDict = Hashtable.Synchronized(new Hashtable(2048));

		private static readonly BindingFlags s_flag = BindingFlags.Instance | BindingFlags.Public;

		public static TypeDescription GetTypeDiscription(Type type)
		{
			TypeDescription typeDescription = s_typeInfoDict[type.FullName] as TypeDescription;
			if (typeDescription == null)
			{
				PropertyInfo[] properties = type.GetProperties(s_flag);
				int capacity = properties.Length;
				Dictionary<string, DbMapInfo> dictionary = new Dictionary<string, DbMapInfo>(capacity, StringComparer.OrdinalIgnoreCase);
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					DbMapInfo dbMapInfo = null;
					DataColumnAttribute myAttribute = propertyInfo.GetMyAttribute<DataColumnAttribute>();
					dbMapInfo = ((myAttribute == null) ? new DbMapInfo(propertyInfo.Name, propertyInfo.Name, null, propertyInfo) : new DbMapInfo(string.IsNullOrEmpty(myAttribute.Alias) ? propertyInfo.Name : myAttribute.Alias, propertyInfo.Name, myAttribute, propertyInfo));
					dictionary[dbMapInfo.DbName] = dbMapInfo;
				}
				typeDescription = new TypeDescription
				{
					MemberDict = dictionary
				};
				s_typeInfoDict[type.FullName] = typeDescription;
			}
			return typeDescription;
		}
	}
}
