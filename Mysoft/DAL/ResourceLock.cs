using System;

namespace XmlToSQL.Mysoft.DAL
{
	public sealed class ResourceLock : IDisposable
	{
		private readonly string _key;

		public ResourceLock(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (key.Length > 255)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			_key = key;
			var parameterObject = new
			{
				Resource = _key,
				LockMode = "Exclusive"
			};
			StoreProcedure storeProcedure = StoreProcedure.Create("sp_getapplock", parameterObject);
			storeProcedure.ExecuteNonQuery();
		}

		public void Dispose()
		{
			var parameterObject = new
			{
				Resource = _key
			};
			StoreProcedure storeProcedure = StoreProcedure.Create("sp_releaseapplock ", parameterObject);
			try
			{
				storeProcedure.ExecuteNonQuery();
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			finally
			{
			}
		}
	}
}
