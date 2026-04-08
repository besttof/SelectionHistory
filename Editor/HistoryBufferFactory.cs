using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	internal interface IBufferSlot
	{
	}

	internal static class HistoryBufferFactory
	{
		internal static IHistoryBuffer<Object[]> Create(int capacity, SelectionMode mode = SelectionMode.FastAndNaive)
		{
			return mode switch
			{
				SelectionMode.FastAndNaive   => new HistoryBuffer<Object[], ObjectSlot>(capacity, new ObjectSlot.Converter()),
				SelectionMode.SlowAndCorrect => new HistoryBuffer<Object[], GlobalObjectIdSlot>(capacity, new GlobalObjectIdSlot.Converter()),
				_                            => throw new ArgumentOutOfRangeException()
			};
		}

		internal static IHistoryBuffer<T> CreateRaw<T>(int capacity)
		{
			return new HistoryBuffer<T, NoSlot<T>>(capacity, new NoSlot<T>.Converter());
		}
	}
}