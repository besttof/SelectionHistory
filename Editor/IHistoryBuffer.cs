using System.Collections.Generic;

namespace Besttof.SelectionHistory
{
	internal interface IHistoryBuffer<T> : IEnumerable<T>
	{
		int Count { get; }
		int Capacity { get; }
		void Clear();
		void Push(T value);
		bool TryGetCurrent(out T value);
		bool TryGoBack(out T value);
		bool TryGoForward(out T value);
	}
}