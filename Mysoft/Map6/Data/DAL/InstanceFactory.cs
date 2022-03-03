using System;
using ClownFish.Base.Reflection;

namespace Mysoft.Map6.Data.DAL
{
	internal static class InstanceFactory
	{
		private class DefaultCreateInstanceProvider : ICreateInstanceProvider
		{
			public object CreateInstance(Type type)
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				return type.FastNew();
			}
		}

		private static ICreateInstanceProvider s_createInstanceProvider = new DefaultCreateInstanceProvider();

		public static void SetInstanceCreateProvider(ICreateInstanceProvider provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			s_createInstanceProvider = provider;
		}

		public static object Create(Type entityType)
		{
			return s_createInstanceProvider.CreateInstance(entityType);
		}

		public static T Create<T>() where T : class
		{
			return (T)Create(typeof(T));
		}
	}
}
