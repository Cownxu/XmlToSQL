using System;

namespace Mysoft.Map6.Data.DAL
{
	public class LockInfo
	{
		public bool Succeeded { get; set; }

		public string LockUser { get; set; }

		public DateTime ExpireTime { get; set; }
	}
}
