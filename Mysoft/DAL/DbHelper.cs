using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using CownxuFils.Base.Reflection;
using CownxuFish.Base.Reflection;

namespace XmlToSQL.Mysoft.DAL
{
	internal static class DbHelper
    {
        public static TransactionStackItem item = new TransactionStackItem();
		internal static int ExecuteNonQuery(DbCommand command)
		{
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(command, (DbCommand cmd) => cmd.ExecuteNonQuery());
		}

		internal static T ExecuteScalar<T>(DbCommand command)
		{
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(command, (DbCommand cmd) => ConvertScalar<T>(cmd.ExecuteScalar()));
		}

		internal static List<T> ToScalarList<T>(DbCommand command)
		{
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(command, delegate(DbCommand cmd)
			{
				List<T> list = new List<T>();
				using DbDataReader dbDataReader = cmd.ExecuteReader();
				while (dbDataReader.Read())
				{
					list.Add(ConvertScalar<T>(dbDataReader[0]));
				}
				return list;
			});
		}

		internal static T ConvertScalar<T>(object obj)
		{
            ConnectionScope._item = item;
			if (obj == null || DBNull.Value.Equals(obj))
			{
				return default(T);
			}
			if (obj is T)
			{
				return (T)obj;
			}
			Type typeFromHandle = typeof(T);
			return (T)Convert.ChangeType(obj, typeFromHandle);
		}

		internal static DbDataReader ExecuteReader(DbCommand command)
		{
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(command, (DbCommand cmd) => cmd.ExecuteReader());
		}

		internal static DataSet ToDataSet(DbCommand command)
		{
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(command, delegate(DbCommand cmd)
			{
				DataSet dataSet = new DataSet();
				DbDataAdapter dbDataAdapter = ProviderManager.CreateDataAdapter();
				dbDataAdapter.SelectCommand = cmd;
				dbDataAdapter.Fill(dataSet);
				for (int i = 0; i < dataSet.Tables.Count; i++)
				{
					dataSet.Tables[i].TableName = "_tb" + i;
				}
				return dataSet;
			});
		}

		internal static DataTable ToDataTable(DbCommand command)
        {
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(command, delegate
			{
				DataTable dataTable = new DataTable("_tb");
				DbDataAdapter dbDataAdapter = ProviderManager.CreateDataAdapter();
				dbDataAdapter.SelectCommand = command;
				dbDataAdapter.Fill(dataTable);
				return dataTable;
			});
		}

		internal static List<T> ToList<T>(DbCommand cmd) where T : class
		{
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(cmd, delegate(DbCommand p)
			{
				using DbDataReader reader = p.ExecuteReader();
				if (typeof(IFillable).IsAssignableFrom(typeof(T)))
				{
					return ToListByIFillable<T>(reader);
				}
				return ToListByReflect<T>(reader);
			});
		}

		private static List<T> ToListByIFillable<T>(DbDataReader reader) where T : class
		{
            ConnectionScope._item = item;
			List<T> list = new List<T>();
			while (reader.Read())
			{
				T val = InstanceFactory.Create<T>();
				IFillable fillable = (IFillable)val;
				fillable.Fill(reader);
				list.Add(val);
			}
			return list;
		}

		private static List<T> ToListByReflect<T>(DbDataReader reader) where T : class
		{
            ConnectionScope._item = item;
			TypeDescription typeDiscription = TypeDescriptionCache.GetTypeDiscription(typeof(T));
			List<T> list = new List<T>();
			string[] columnNames = GetColumnNames(reader);
			while (reader.Read())
			{
				T item = ReaderToObject<T>(reader, typeDiscription, columnNames);
				list.Add(item);
			}
			return list;
		}

		internal static T ToSingle<T>(DbCommand cmd) where T : class
		{
            ConnectionScope._item = item;
			using ConnectionScope connectionScope = new ConnectionScope();
			return connectionScope.Current.ExecuteCommand(cmd, delegate(DbCommand p)
			{
				using DbDataReader reader = p.ExecuteReader();
				if (typeof(IFillable).IsAssignableFrom(typeof(T)))
				{
					return ToSingleByIFillable<T>(reader);
				}
				return ToSingleByReflect<T>(reader);
			});
		}

		private static T ToSingleByIFillable<T>(DbDataReader reader) where T : class
		{
            ConnectionScope._item = item;
			if (reader.Read())
			{
				T val = InstanceFactory.Create<T>();
				IFillable fillable = (IFillable)val;
				fillable.Fill(reader);
				return val;
			}
			return null;
		}

		private static T ToSingleByReflect<T>(DbDataReader reader) where T : class
		{
			TypeDescription typeDiscription = TypeDescriptionCache.GetTypeDiscription(typeof(T));
			if (reader.Read())
			{
				string[] columnNames = GetColumnNames(reader);
				return ReaderToObject<T>(reader, typeDiscription, columnNames);
			}
			return null;
		}

		private static T ReaderToObject<T>(DbDataReader reader, TypeDescription description, string[] names) where T : class
		{
			Dictionary<string, DbMapInfo> memberDict = description.MemberDict;
			T val = (T)typeof(T).FastNew();
			for (int i = 0; i < names.Length; i++)
			{
				string text = names[i];
				if (memberDict.TryGetValue(text, out var value))
				{
					object value2 = reader.GetValue(i);
					val.ReflectSetValue(value, value2, text);
				}
			}
			return val;
		}

		internal static string[] GetColumnNames(DbDataReader reader)
		{
			int fieldCount = reader.FieldCount;
			string[] array = new string[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				array[i] = reader.GetName(i);
			}
			return array;
		}
	}
}
