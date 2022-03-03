using System;
using System.Runtime.CompilerServices;
using Mysoft.Map6.Data.DAL;

namespace Mysoft.Map6.Data
{
	internal static class Initializer
	{
		private static bool s_inited = false;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void UnSafeInit(string connectionString, string providerName)
		{
			if (s_inited)
			{
				throw new InvalidOperationException("请不要多次调用UnSafeInit方法!");
			}
			ConnectionScope.SetDefaultConnection(connectionString, providerName);
			s_inited = true;
		}
	}
}
