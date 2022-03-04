using System;
using System.Collections;

namespace Mysoft.Map6.Data.Test
{
	public static class MockResult
	{
		[ThreadStatic]
		private static Queue s_queue;

		public static void PushResult(object result)
		{
			if (s_queue == null)
			{
				s_queue = new Queue(10);
			}
			s_queue.Enqueue(result);
		}

		public static void Clear()
		{
			if (s_queue != null)
			{
				s_queue.Clear();
			}
		}

		internal static object GetResult()
		{
			if (s_queue == null)
			{
				return null;
			}
			if (s_queue.Count > 0)
			{
				return s_queue.Dequeue();
			}
			return null;
		}
	}
}
