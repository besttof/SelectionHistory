namespace Besttof.SelectionHistory
{
	internal interface IBufferSlot<T>
	{
		public void Set(T value);
		public T Get();
	}
}