using System;
using System.Collections;
using System.Data.Common;

namespace XmlToSQL.Mysoft.DAL
{
	public class CommandEventArgs : EventArgs
	{
		private Hashtable _userData;

		public DbCommand Command { get; internal set; }

		public Hashtable UserData
		{
			get
			{
				if (_userData == null)
				{
					_userData = new Hashtable();
				}
				return _userData;
			}
			internal set
			{
				_userData = value;
			}
		}

		internal Hashtable InternalGetUserData()
		{
			return _userData;
		}
	}
}
