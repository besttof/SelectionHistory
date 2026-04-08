using System;
using System.Runtime.CompilerServices;

namespace Besttof.SelectionHistory
{
	[Serializable]
	internal class NoSlot<T> : IBufferSlot
	{
		public T Value;

		[Serializable]
		internal class Converter : IBufferConverter<T, NoSlot<T>>
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public NoSlot<T> ToBuffer(T value) => new() { Value = value };

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public T FromBuffer(NoSlot<T> slot) => slot.Value;
		}
	}
}