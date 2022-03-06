using CownxuFish.Base.Reflection;
using System;
using System.Collections.Generic;
using System.Data;

namespace XmlToSQL.Mysoft.DAL
{
    public static class DataTableExtensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (typeof(IFillable).IsAssignableFrom(typeof(T)))
            {
                return ToListByIFillable<T>(table);
            }
            return ToListByReflect<T>(table);
        }

        internal static List<T> ToListByReflect<T>(DataTable table) where T : class
        {
            TypeDescription typeDiscription = TypeDescriptionCache.GetTypeDiscription(typeof(T));
            Dictionary<string, DbMapInfo> memberDict = typeDiscription.MemberDict;
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T val = (T)typeof(T).FastNew();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    string columnName = table.Columns[i].ColumnName;
                    if (memberDict.TryGetValue(columnName, out var value))
                    {
                        object val2 = row[i];
                        val.ReflectSetValue(value, val2, columnName);
                    }
                }
                list.Add(val);
            }
            return list;
        }

        internal static List<T> ToListByIFillable<T>(DataTable table) where T : class
        {
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T val = InstanceFactory.Create<T>();
                IFillable fillable = (IFillable)val;
                fillable.Fill(row);
                list.Add(val);
            }
            return list;
        }
    }
}
