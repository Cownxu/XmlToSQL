using System;

namespace XmlToSQL.Mysoft.DAL
{
	internal static class ConvertExtensions
	{
		internal static object ConvertToType(this object value, Type targetType)
		{
			if (value == null)
			{
				return null;
			}
			if (targetType == typeof(string))
			{
				return value.ToString();
			}
			Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
			if (value.GetType() == type)
			{
				return value;
			}
			if (type == typeof(Guid) && value is string)
			{
				return new Guid(value.ToString());
			}
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			return Convert.ChangeType(value, type);
		}

		public static object ConvertToTimeStamp(this object value, Type targetType)
		{
			if (value == null)
			{
				return null;
			}
			if (value.GetType() == targetType)
			{
				return value;
			}
			if (value.GetType() == typeof(byte[]) && targetType == typeof(long))
			{
				byte[] array = (byte[])value;
				return array.TimeStampToInt64();
			}
			if (value is long && targetType == typeof(byte[]))
			{
				long value2 = (long)value;
				return value2.Int64ToTimeStamp();
			}
			return Convert.ChangeType(value, targetType);
		}

		private static byte[] ReverseArray(byte[] oldArray)
		{
			byte[] array = new byte[oldArray.Length];
			int num = oldArray.Length - 1;
			for (int i = 0; i < oldArray.Length; i++)
			{
				array[num] = oldArray[i];
				num--;
			}
			return array;
		}

		public static long TimeStampToInt64(this byte[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length != 8)
			{
				throw new ArgumentOutOfRangeException("array", "byte[]数组表示的时间戳，长度应该为8。");
			}
			return BitConverter.ToInt64(ReverseArray(array), 0);
		}

		public static byte[] Int64ToTimeStamp(this long value)
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value", "long类型表示的时间戳应该大于0。");
			}
			return ReverseArray(BitConverter.GetBytes(value));
		}
	}
}
