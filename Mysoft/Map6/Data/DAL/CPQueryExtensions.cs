namespace Mysoft.Map6.Data.DAL
{
	public static class CPQueryExtensions
	{
		public static CPQuery AsCPQuery(this string s)
		{
			return new CPQuery(s);
		}

		public static QueryParameter AsQueryParameter(this string b)
		{
			return new QueryParameter(b);
		}

		public static SqlFragment AsSql(this string s)
		{
			return new SqlFragment(s);
		}
	}
}
