using System;

namespace Mysoft.Map6.Data.DAL
{
	internal interface ICreateInstanceProvider
	{
		object CreateInstance(Type type);
	}
}
