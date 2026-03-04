using System;
using System.Runtime.CompilerServices;

namespace Besttof.SelectionHistory
{
	internal class NoConverter<T> : IBufferConverter<T, T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ToBuffer(T value) => value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T FromBuffer(T bufferValue) => bufferValue;
	}
}