using System.Data;
using System.Data.Common;

namespace XmlToSQL.Mysoft.DAL
{
	public interface IFillable
	{
		void Fill(DbDataReader reader);

		void Fill(DataRow dataRow);
	}
}
