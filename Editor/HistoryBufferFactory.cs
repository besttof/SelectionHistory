using System;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	internal static class HistoryBufferFactory
	{
		internal static IHistoryBuffer<Object[]> Create(int capacity, SelectionMode mode = SelectionMode.FastAndNaive)
		{
			return mode switch
			{
				SelectionMode.FastAndNaive   => new HistoryBuffer<Object[], ObjectSlot>(capacity),
				SelectionMode.SlowAndCorrect => new HistoryBuffer<Object[], GlobalObjectIdSlot>(capacity),
				_                            => throw new ArgumentOutOfRangeException()
			};
		}
	}
}