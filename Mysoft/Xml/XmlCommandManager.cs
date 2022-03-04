using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using ClownFish.Base;
using ClownFish.Base.Xml;

namespace XmlToSQL.Mysoft.Xml
{
	public static class XmlCommandManager
	{
		private static readonly string s_CacheKey = Guid.NewGuid().ToString();

		private static Exception s_ExceptionOnLoad = null;

		private static Dictionary<string, XmlCommandItem> s_dict = null;

		private static Dictionary<string, XmlCommandItem> s_dictXml = new Dictionary<string, XmlCommandItem>();

		//private static readonly bool s_IsAspnetEnvironment = HttpRuntime.AppDomainAppId != null;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void LoadCommnads(string directoryPath)
		{
			if (s_dict != null)//&& s_IsAspnetEnvironment
			{
				throw new InvalidOperationException("不允许重复调用这个方法。");
			}
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException($"目录 {directoryPath} 不存在。");
			}
			Exception exception = null;
			s_dict = LoadCommandsInternal(directoryPath, out exception);
			if (exception != null)
			{
				s_ExceptionOnLoad = exception;
			}
			if (s_ExceptionOnLoad != null)
			{
				throw s_ExceptionOnLoad;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void LoadXml(string xml)
		{
			if (string.IsNullOrEmpty(xml))
			{
				throw new ArgumentNullException("xml");
			}
			List<XmlCommandItem> list = xml.FromXml<List<XmlCommandItem>>();
			list.ForEach(delegate(XmlCommandItem p)
			{
				if (!s_dictXml.TryGetValue(p.CommandName, out var _))
				{
					s_dictXml.AddValue(p.CommandName, p);
				}
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void LoadCustomizeXml(string xmlContent)
		{
			List<XmlCommandItem> list = xmlContent.FromXml<List<XmlCommandItem>>();
			list.ForEach(delegate(XmlCommandItem x)
			{
				s_dictXml.UpdateValue(x.CommandName, x);
			});
		}

		private static Dictionary<string, XmlCommandItem> LoadCommandsInternal(string directoryPath, out Exception exception)
		{
			exception = null;
			Dictionary<string, XmlCommandItem> dictionary = null;
			try
			{
				string[] files = Directory.GetFiles(directoryPath, "*.config", SearchOption.AllDirectories);
				if (files.Length != 0)
				{
					dictionary = new Dictionary<string, XmlCommandItem>(2048);
					string[] array = files;
					foreach (string path in array)
					{
						List<XmlCommandItem> list = XmlHelper.XmlDeserializeFromFile<List<XmlCommandItem>>(path);
						foreach (XmlCommandItem item in list)
						{
							dictionary.AddValue(item.CommandName, item);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = (exception = ex);
				dictionary = null;
			}

            //Environment.GetEnvironmentVariable("Aspnet");
			//if (s_IsAspnetEnvironment)
			//{
			//	CacheDependency dependencies = new CacheDependency(directoryPath);
			//	HttpRuntime.Cache.Insert(s_CacheKey, directoryPath, dependencies, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, CacheRemovedCallback);
			//}
			return dictionary;
		}

		private static void CacheRemovedCallback(string key, object value)
		{
			Exception exception = null;
			string directoryPath = (string)value;
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(3000);
				Dictionary<string, XmlCommandItem> dictionary = LoadCommandsInternal(directoryPath, out exception);
				if (exception == null)
				{
					try
					{
					}
					finally
					{
						s_dict = dictionary;
						s_ExceptionOnLoad = null;
					}
					return;
				}
			}
			if (exception != null)
			{
				s_ExceptionOnLoad = exception;
			}
		}

		public static XmlCommandItem GetCommand(string name)
		{
			if (s_ExceptionOnLoad != null)
			{
				throw s_ExceptionOnLoad;
			}
			if (s_dictXml != null && s_dictXml.TryGetValue(name, out var value))
			{
				return value;
			}
			if (s_dict != null && s_dict.TryGetValue(name, out value))
			{
				return value;
			}
			return null;
		}
	}
}
