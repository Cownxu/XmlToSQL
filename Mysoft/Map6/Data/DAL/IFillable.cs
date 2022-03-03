using System.Data;
using System.Data.Common;

namespace Mysoft.Map6.Data.DAL
{
	public interface IFillable
	{
		void Fill(DbDataReader reader);

		void Fill(DataRow dataRow);
	}
}
