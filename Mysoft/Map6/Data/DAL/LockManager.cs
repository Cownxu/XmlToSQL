using System;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
using Mysoft.Map6.Data.Xml;

namespace Mysoft.Map6.Data.DAL
{
	public static class LockManager
	{
		private static readonly object s_lock = new object();

		private static bool s_inited = false;

		private static void Init()
		{
			if (s_inited)
			{
				return;
			}
			lock (s_lock)
			{
				if (!s_inited)
				{
					LoadXmlCommand();
					s_inited = true;
				}
			}
		}

		private static void LoadXmlCommand()
		{
			string text = $"{Assembly.GetExecutingAssembly().GetName().Name}.Res.LockManager.config";
			try
			{
				using Stream stream = typeof(LockManager).Assembly.GetManifestResourceStream(text);
				StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
				string xml = streamReader.ReadToEnd();
				XmlCommandManager.LoadXml(xml);
			}
			catch (InvalidOperationException inner)
			{
				throw new Exception($"加载XmlCommand失败。请检查{typeof(LockManager).Assembly.FullName} - {text}。", inner);
			}
		}

		public static void ReleaseLock(string oid, string type, string user)
		{
			if (string.IsNullOrEmpty(oid))
			{
				throw new ArgumentNullException("oid");
			}
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentNullException("type");
			}
			Init();
			var argsObject = new
			{
				LockKey = oid,
				LockType = type
			};
			LockInfo lockInfo = XmlCommand.From("MAP2:LockManager:GetLock", argsObject).ToSingle<LockInfo>();
			if (lockInfo != null)
			{
				if (!(lockInfo.LockUser == user) && !string.IsNullOrEmpty(user))
				{
					throw new InvalidOperationException($"无法释放由：{lockInfo.LockUser}获得的锁。");
				}
				var argsObject2 = new
				{
					LockKey = oid,
					LockType = type
				};
				XmlCommand.From("MAP2:LockManager:DelLock", argsObject2).ExecuteNonQuery();
			}
		}

		public static LockInfo GetLock(string oid, string type, string user, TimeSpan expire)
		{
			if (string.IsNullOrEmpty(oid))
			{
				throw new ArgumentNullException("oid");
			}
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(user))
			{
				throw new ArgumentNullException("user");
			}
			Init();
			LockInfo lockInfo = null;
			string providerName = ConnectionScope.ProviderName;
			string connectionString = ConnectionScope.ConnectionString;
			DateTime dateTime = XmlCommand.From("MAP2:LockManager:GetDbDateTime:" + providerName).ExecuteScalar<DateTime>();
			DateTime expireTime = dateTime + expire;
			using (new ConnectionScope(TransactionMode.Inherits, connectionString + " ", providerName))
			{
				var argsObject = new
				{
					LockKey = oid,
					LockType = type,
					LockUser = user,
					ExpireTime = expireTime
				};
				lockInfo = GetLockInfo(oid, type);
				if (lockInfo == null)
				{
					try
					{
						XmlCommand.From("MAP2:LockManager:AddLock:" + providerName, argsObject).ExecuteNonQuery();
					}
					catch (DbException)
					{
						lockInfo = GetLockInfo(oid, type);
						lockInfo.Succeeded = false;
					}
				}
				if (lockInfo == null)
				{
					lockInfo = new LockInfo();
					lockInfo.Succeeded = true;
					lockInfo.ExpireTime = expireTime;
					lockInfo.LockUser = user;
				}
				else if (lockInfo.ExpireTime > dateTime)
				{
					if (lockInfo.LockUser == user)
					{
						lockInfo.Succeeded = true;
						var argsObject2 = new
						{
							ExpireTime = expireTime,
							LockKey = oid,
							LockType = type
						};
						XmlCommand.From("MAP2:LockManager:UpdateExpireTime", argsObject2).ExecuteNonQuery();
					}
					else
					{
						lockInfo.Succeeded = false;
					}
				}
				else
				{
					var argsObject3 = new
					{
						LockUser = user,
						CreateTime = dateTime,
						ExpireTime = expireTime,
						LockKey = oid,
						LockType = type
					};
					XmlCommand.From("MAP2:LockManager:UpdateLock", argsObject3).ExecuteNonQuery();
					lockInfo.Succeeded = true;
					lockInfo.ExpireTime = expireTime;
					lockInfo.LockUser = user;
				}
			}
			return lockInfo;
		}

		private static LockInfo GetLockInfo(string oid, string type)
		{
			Init();
			var argsObject = new
			{
				LockKey = oid,
				LockType = type
			};
			return XmlCommand.From("MAP2:LockManager:GetLock", argsObject).ToSingle<LockInfo>();
		}

		public static LockInfo GetLock(string oid, string type, string user)
		{
			Init();
			TimeSpan expire = new TimeSpan(73000, 0, 0, 0, 0);
			return GetLock(oid, type, user, expire);
		}
	}
}
