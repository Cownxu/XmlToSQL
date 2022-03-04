using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace XmlToSQL.Mysoft.DAL
{
	public interface IDbExecute
	{
		int ExecuteNonQuery();

		T ExecuteScalar<T>();

		DbDataReader ExecuteReader();

		DataSet ToDataSet();

		DataTable ToDataTable();

		List<T> ToScalarList<T>();

		List<T> ToList<T>() where T : class;

		T ToSingle<T>() where T : class;
	}
}
