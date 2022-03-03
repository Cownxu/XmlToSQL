using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using ClownFish.Base.Reflection;

namespace Mysoft.Map6.Data.DAL
{
	[Obsolete]
	public sealed class StoreProcedure : IDbExecute
	{
		public DbCommand Command { get; private set; }

		public StoreProcedure(string spName, params DbParameter[] parameters)
		{
			if (string.IsNullOrEmpty(spName))
			{
				throw new ArgumentNullException("spName");
			}
			Command = ProviderManager.CreateCommand();
			Command.CommandText = spName;
			Command.CommandType = CommandType.StoredProcedure;
			if (parameters != null)
			{
				foreach (DbParameter value in parameters)
				{
					Command.Parameters.Add(value);
				}
			}
		}

		public StoreProcedure(string spName, object parameterObject)
			: this(spName, GetSpParameters(parameterObject))
		{
		}

		public StoreProcedure(string spName)
			: this(spName, (DbParameter[])null)
		{
		}

		public static StoreProcedure Create(string spName)
		{
			return new StoreProcedure(spName, (DbParameter[])null);
		}

		public static StoreProcedure Create(string spName, object parameterObject)
		{
			return new StoreProcedure(spName, parameterObject);
		}

		public static StoreProcedure Create(string spName, params DbParameter[] parameters)
		{
			return new StoreProcedure(spName, parameters);
		}

		internal static DbParameter[] GetSpParameters(object parameterObject)
		{
			if (parameterObject == null)
			{
				return null;
			}
			PropertyInfo[] properties = parameterObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			DbParameter[] array = new DbParameter[properties.Length];
			int num = 0;
			PropertyInfo[] array2 = properties;
			foreach (PropertyInfo propertyInfo in array2)
			{
				string name = "@" + propertyInfo.Name;
				object obj = propertyInfo.FastGetValue(parameterObject);
				DbParameter dbParameter = null;
				if (obj == null || obj == DBNull.Value)
				{
					dbParameter = ProviderManager.CreateParameter(name, DBNull.Value);
				}
				else
				{
					DbParameter dbParameter2 = obj as DbParameter;
					dbParameter = ((dbParameter2 == null) ? ProviderManager.CreateParameter(name, obj) : dbParameter2);
				}
				array[num] = dbParameter;
				num++;
			}
			return array;
		}

		public int ExecuteNonQuery()
		{
			return DbHelper.ExecuteNonQuery(Command);
		}

		public DbDataReader ExecuteReader()
		{
			return DbHelper.ExecuteReader(Command);
		}

		public DataTable ToDataTable()
		{
			return DbHelper.ToDataTable(Command);
		}

		public DataSet ToDataSet()
		{
			return DbHelper.ToDataSet(Command);
		}

		public T ExecuteScalar<T>()
		{
			return DbHelper.ExecuteScalar<T>(Command);
		}

		public List<T> ToScalarList<T>()
		{
			return DbHelper.ToScalarList<T>(Command);
		}

		public List<T> ToList<T>() where T : class
		{
			return DbHelper.ToList<T>(Command);
		}

		public T ToSingle<T>() where T : class
		{
			return DbHelper.ToSingle<T>(Command);
		}
	}
}
