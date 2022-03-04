using System;
using System.Collections.Generic;

namespace XmlToSQL.Mysoft.Xml
{
	public static class DictionaryExtensions
	{
		internal static void AddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			try
			{
				dict.Add(key, value);
			}
			catch (ArgumentException innerException)
			{
				throw new ArgumentException($"往集合中插入元素时发生了异常，当前Key={key}*", innerException);
			}
		}

		internal static void UpdateValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			try
			{
				if (dict.TryGetValue(key, out var _))
				{
					dict[key] = value;
				}
				else
				{
					dict.Add(key, value);
				}
			}
			catch (ArgumentException innerException)
			{
				throw new ArgumentException($"更新集合中元素时发生了异常，当前Key={key}", innerException);
			}
		}
	}
}
