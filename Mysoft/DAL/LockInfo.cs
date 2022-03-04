using System;

namespace XmlToSQL.Mysoft.DAL
{
	public class LockInfo
	{
		public bool Succeeded { get; set; }

		public string LockUser { get; set; }

		public DateTime ExpireTime { get; set; }
	}
}
