namespace Besttof.SelectionHistory
{
	internal interface IBufferConverter<T, TBuffer>
	{
		public TBuffer ToBuffer(T value);

		public T FromBuffer(TBuffer bufferValue);
	}
}