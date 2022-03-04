using System;

namespace XmlToSQL.Mysoft.DAL
{
	internal interface ICreateInstanceProvider
	{
		object CreateInstance(Type type);
	}
}
