using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using CownxuFish.Base.Reflection;

namespace XmlToSQL.Mysoft.DAL
{
	[Serializable]
	public sealed class CPQuery : IDbExecute
	{
		private enum SPStep
		{
			NotSet,
			EndWith,
			Skip
		}
        public static TransactionStackItem item = new TransactionStackItem();
		private static int s_index;

		private StringBuilder _sb = new StringBuilder(512);

		[NonSerialized]
		private DbCommand _command = ProviderManager.CreateCommand();

		public DbCommand Command => _command;

		internal DbCommand GetCommand()
		{
			_command.CommandText = _sb.ToString();
			return _command;
		}

		internal CPQuery(string text)
		{
			AddSqlText(text);
            DbHelper.item = item;
        }

		public static CPQuery Create()
		{
			return new CPQuery(null);
		}

		public override string ToString()
		{
			return _sb.ToString();
		}

		private void AddSqlText(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				_sb.Append(s);
			}
		}

		internal static void ResetParameterIndex()
		{
			s_index = 0;
		}

		private static uint GetNextParamIndex()
		{
			int num = Interlocked.Increment(ref s_index);
			return (uint)((num >= 0) ? num : (uint.MaxValue + num + 1));
		}

		private void AddParameter(QueryParameter p)
		{
			string text = "@p" + GetNextParamIndex();
			_sb.Append(text);
			DbParameter value = ProviderManager.CreateParameter(text, p.Value);
			_command.Parameters.Add(value);
		}

		public static CPQuery From(string parameterizedSQL, object argsObject)
		{
			if (string.IsNullOrEmpty(parameterizedSQL))
			{
				throw new ArgumentNullException("parameterizedSQL");
			}
			CPQuery cPQuery = new CPQuery(parameterizedSQL);
			if (argsObject != null)
			{
				PropertyInfo[] properties = argsObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					object value = propertyInfo.FastGetValue(argsObject);//.FastGetValue()
					SetParameter(cPQuery, propertyInfo.Name, value);
				}
			}
			return cPQuery;
		}

		public static CPQuery From(string parameterizedSQL, Dictionary<string, object> dictionary)
		{
			if (string.IsNullOrEmpty(parameterizedSQL))
			{
				throw new ArgumentNullException("parameterizedSQL");
			}
			CPQuery cPQuery = new CPQuery(parameterizedSQL);
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					SetParameter(cPQuery, item.Key, item.Value);
				}
			}
			return cPQuery;
		}

		private static void SetParameter(CPQuery query, string name, object value)
		{
			string name2 = "@" + name;
			if (value == null || value == DBNull.Value)
			{
				DbParameter value2 = ProviderManager.CreateParameter(name2, DBNull.Value);
				query._command.Parameters.Add(value2);
			}
			else if (value is DbParameter)
			{
				query._command.Parameters.Add(value as DbParameter);
			}
			else if (value is ICollection)
			{
				name2 = "{" + name + "}";
				SetInArrayParameters(query, name2, (ICollection)value);
			}
			else if (value is CPQuery)
			{
				name2 = "{" + name + "}";
				SetQueryParameter(query, name2, (CPQuery)value);
			}
			else if (value is SqlFragment)
			{
				name2 = "{" + name + "}";
				SqlFragment sqlFragment = (SqlFragment)value;
				query._sb.Replace(name2, sqlFragment.Value);
			}
			else
			{
				DbParameter value3 = ProviderManager.CreateParameter(name2, value);
				query._command.Parameters.Add(value3);
			}
		}

		internal void MoveParameters(DbCommand command)
		{
			DbParameter[] values = _command.Parameters.Cast<DbParameter>().ToArray();
			_command.Parameters.Clear();
			command.Parameters.AddRange(values);
		}

		private static void SetQueryParameter(CPQuery query, string pname, CPQuery subQuery)
		{
			query._sb.Replace(pname, subQuery.ToString());
			subQuery.MoveParameters(query._command);
		}

		private static void SetInArrayParameters(CPQuery query, string pname, ICollection collection)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			ArrayToString(collection, stringBuilder);
			if (stringBuilder.Length == 0)
			{
				foreach (object item in collection)
				{
					string text = "@x" + GetNextParamIndex();
					DbParameter value = ProviderManager.CreateParameter(text, item);
					query._command.Parameters.Add(value);
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(text);
				}
			}
			if (stringBuilder.Length == 0)
			{
				stringBuilder.Append("NULL");
			}
			query._sb.Replace(pname, stringBuilder.ToString());
		}

		internal static void ArrayToString(ICollection collection, StringBuilder sb)
		{
			if (typeof(IEnumerable<int>).IsAssignableFrom(collection.GetType()))
			{
				foreach (object item in collection)
				{
					if (sb.Length > 0)
					{
						sb.Append(',');
					}
					sb.Append(item.ToString());
				}
			}
			else
			{
				if (!typeof(IEnumerable<Guid>).IsAssignableFrom(collection.GetType()))
				{
					return;
				}
				foreach (object item2 in collection)
				{
					if (sb.Length > 0)
					{
						sb.Append(',');
					}
					sb.Append("'").Append(item2.ToString()).Append("'");
				}
			}
		}

		public static CPQuery From(string parameterizedSQL, params DbParameter[] parameters)
		{
			CPQuery cPQuery = new CPQuery(parameterizedSQL);
			if (parameters != null)
			{
				foreach (DbParameter value in parameters)
				{
					cPQuery._command.Parameters.Add(value);
				}
			}
			return cPQuery;
		}

		public static CPQuery From(string parameterizedSQL)
		{
			return From(parameterizedSQL, (object)null);
		}

		public static CPQuery operator +(CPQuery query, string s)
		{
			query.AddSqlText(s);
			return query;
		}

		public static CPQuery operator +(CPQuery query, SqlFragment s)
		{
			query.AddSqlText(s.Value);
			return query;
		}

		public static CPQuery operator +(CPQuery query, CPQuery query2)
		{
			query.AddSqlText(query2.ToString());
			query2.MoveParameters(query._command);
			return query;
		}

		public static CPQuery operator +(CPQuery query, QueryParameter p)
		{
			query.AddParameter(p);
			return query;
		}

		public static CPQuery operator +(CPQuery query, DbParameter p)
		{
			query.AddSqlText(p.ParameterName);
			query._command.Parameters.Add(p);
			return query;
		}

		public int ExecuteNonQuery()
		{
			return DbHelper.ExecuteNonQuery(GetCommand());
		}

		public DataTable ToDataTable()
		{
			return DbHelper.ToDataTable(GetCommand());
		}

		public DataSet ToDataSet()
		{
			return DbHelper.ToDataSet(GetCommand());
		}

		public DbDataReader ExecuteReader()
		{
			return DbHelper.ExecuteReader(GetCommand());
		}

		public T ExecuteScalar<T>()
		{
			return DbHelper.ExecuteScalar<T>(GetCommand());
		}

		public List<T> ToScalarList<T>()
		{
			return DbHelper.ToScalarList<T>(GetCommand());
		}

		public List<T> ToList<T>() where T : class
		{
			return DbHelper.ToList<T>(GetCommand());
		}

		public T ToSingle<T>() where T : class
		{
			return DbHelper.ToSingle<T>(GetCommand());
		}
	}
}
