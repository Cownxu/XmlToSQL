using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CownxuFils.Base.Reflection;
using CownxuFish.Base.Reflection;
using XmlToSQL.Mysoft.Xml;

namespace XmlToSQL.Mysoft.DAL
{
	public sealed class XmlCommand : IXmlCommand
	{

        public class PagingInfo
		{
			public int PageIndex { get; set; }

			public int PageSize { get; set; }

			public int TotalRecords { get; set; }

			public int CalcPageCount()
			{
				if (PageSize == 0 || TotalRecords == 0)
				{
					return 0;
				}
				return (int)Math.Ceiling((double)TotalRecords / (double)PageSize);
			}
		}

        #region 成员变量

        private readonly CPQuery? _query;

        private int _paramIndex = 1;

        private static readonly Regex s_pagingRegex = new Regex("\\)\\s*as\\s*rowindex\\s*,", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        internal CPQuery Query => _query;

        public DbCommand Command => _query!.Command;

        #endregion

		//public XmlCommand(string name)
		//	: this(name, null)
		//{
		//}
        public XmlCommand(TransactionStackItem item)
        {
            CPQuery.item = item;

		}
		public XmlCommand(string name, object argsObject)
		{
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            XmlCommandItem command = XmlCommandManager.GetCommand(name);
            if (command == null)
            {
                throw new ArgumentOutOfRangeException("name", $"指定的XmlCommand名称 {name} 不存在。");
            }
            _query = BuildCPQuery(command, argsObject);
            if (command.Timeout > _query.Command.CommandTimeout)
            {
                _query.Command.CommandTimeout = command.Timeout;
            }
            _query.Command.CommandType = command.CommandType;
		}

  //      private void ExecFrom(string name, object argsObject)
  //      {
           
		//}

        public async Task<XmlCommand> From(string name)
		{
            Task<XmlCommand> command = Task.Run<XmlCommand>(() => { return new XmlCommand(name,null); });
            return await command;
		}

		public async Task<XmlCommand> From(string name, object argsObject)
        {
            Task<XmlCommand> command =  Task.Run<XmlCommand>(() => { return new XmlCommand(name, argsObject); });
			return await command;
		}
        internal static XmlCommand FromPri(string name)
        {
            return new XmlCommand(name,null);
        }

        internal static XmlCommand FromPri(string name, object argsObject)
        {
            return new XmlCommand(name, argsObject);
        }
		private List<DbParameter> CloneParameters(XmlCommandItem command)
		{
			List<DbParameter> list = new List<DbParameter>(command.Parameters.Count);
			foreach (XmlCmdParameter parameter in command.Parameters)
			{
				DbParameter dbParameter = ProviderManager.CreateParameter();
				dbParameter.ParameterName = parameter.Name;
				dbParameter.DbType = parameter.Type;
				dbParameter.Direction = parameter.Direction;
				dbParameter.Size = parameter.Size;
				list.Add(dbParameter);
			}
			return list;
		}

		private CPQuery BuildCPQuery(XmlCommandItem command, object argsObject)
		{
			if (argsObject == null)
			{
				return new CPQuery(command.CommandText);
			}
			string text = command.CommandText;
			List<DbParameter> list = CloneParameters(command);
			PropertyInfo[] properties = argsObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				string name = "@" + propertyInfo.Name;
				object obj = propertyInfo.FastGetValue(argsObject) ?? DBNull.Value;
				DbParameter dbParameter = list.FirstOrDefault((DbParameter x) => string.Compare(x.ParameterName, name, StringComparison.OrdinalIgnoreCase) == 0);
				if (dbParameter != null)
				{
					dbParameter.Value = obj;
					continue;
				}
				string text2 = "{" + propertyInfo.Name + "}";
				int num = text.IndexOf(text2, StringComparison.OrdinalIgnoreCase);
				if (num > 0)
				{
					text = ((!(obj is CPQuery)) ? ((!(obj is ICollection)) ? text.Replace(text2, obj.ToString()) : SetInArrayParameter(text, list, text2, (ICollection)obj)) : SetQueryParameter(text, list, text2, (CPQuery)obj));
				}
			}
			return CPQuery.From(text, list.ToArray());
		}

		private string SetInArrayParameter(string commandText, List<DbParameter> parameters, string placeholder, ICollection collection)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			CPQuery.ArrayToString(collection, stringBuilder);
			if (stringBuilder.Length == 0)
			{
				foreach (object item in collection)
				{
					string text = "x" + _paramIndex++;
					DbParameter dbParameter = ProviderManager.CreateParameter();
					dbParameter.ParameterName = "@" + text;
					dbParameter.Value = item;
					parameters.Add(dbParameter);
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append("@" + text);
				}
			}
			if (stringBuilder.Length == 0)
			{
				stringBuilder.Append("NULL");
			}
			return commandText.Replace(placeholder, stringBuilder.ToString());
		}

		private string SetQueryParameter(string commandText, List<DbParameter> parameters, string placeholder, CPQuery query)
		{
			DbCommand command = query.Command;
			DbParameter[] collection = command.Parameters.Cast<DbParameter>().ToArray();
			command.Parameters.Clear();
			parameters.AddRange(collection);
			return commandText.Replace(placeholder, query.ToString());
		}

		private DbParameter[] DepthCopy(DbParameter[] parameters, DbCommand command)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			List<DbParameter> list = new List<DbParameter>(parameters.Length);
			foreach (DbParameter dbParameter in parameters)
			{
				DbParameter dbParameter2 = command.CreateParameter();
				dbParameter2.DbType = dbParameter.DbType;
				dbParameter2.Direction = dbParameter.Direction;
				dbParameter2.IsNullable = dbParameter.IsNullable;
				dbParameter2.ParameterName = dbParameter.ParameterName;
				dbParameter2.Size = dbParameter.Size;
				dbParameter2.Value = dbParameter.Value;
				list.Add(dbParameter2);
			}
			return list.ToArray();
		}

		[Obsolete]
		public int ExecuteNonQuery()
		{
			return _query.ExecuteNonQuery();
		}

		[Obsolete]
		public DbDataReader ExecuteReader()
		{
			return _query.ExecuteReader();
		}

		public T ExecuteScalar<T>()
		{
			return _query.ExecuteScalar<T>();
		}

		public DataSet ToDataSet()
		{
			return _query.ToDataSet();
		}

		public DataTable ToDataTable()
		{
			return _query.ToDataTable();
		}

		public List<T> ToScalarList<T>()
		{
			return _query.ToScalarList<T>();
		}

		public List<T> ToList<T>() where T : class
		{
			return _query.ToList<T>();
		}

		public T ToSingle<T>() where T : class
		{
			return _query.ToSingle<T>();
		}
	}
}
