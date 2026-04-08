using System;

namespace Besttof.SelectionHistory
{
	[Serializable]
	internal struct ValueSlot<T> : IBufferSlot<T>
	{
		public T Value;

		public void Set(T value) => Value = value;
		public T Get() => Value;
	}
}