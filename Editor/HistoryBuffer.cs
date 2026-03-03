using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Besttof.SelectionHistory
{
	internal class HistoryBuffer<T> : IEnumerable<T>
	{
		private readonly T[] _buffer;
		private int _head;
		private int _count;
		private int _cursor;

		internal int Count => _count;
		internal int Capacity => _buffer.Length;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int PhysicalIndex(int logical) => (_head + logical) % Capacity;

		internal HistoryBuffer(int capacity)
		{
			_buffer = new T[capacity];
			Clear();
		}

		internal void Clear()
		{
			_head = 0;
			_count = 0;
			_cursor = -1;

			Array.Fill(_buffer, default);
		}

		internal void Push(T value)
		{
			// Truncate the count to the cursor position
			_count = _cursor + 1;

			if (_count < Capacity)
			{
				_buffer[PhysicalIndex(_count)] = value;
				_count++;
			}
			else
			{
				// When at capacity, overwrite the oldest value and move the head forward
				_buffer[PhysicalIndex(0)] = value;
				_head = PhysicalIndex(1);
			}

			_cursor = _count - 1;
		}

		internal bool TryGetCurrent(out T value)
		{
			value = default;
			if (_count == 0) return false;

			value = _buffer[PhysicalIndex(_cursor)];
			return true;
		}

		internal bool TryGoBack(out T value)
		{
			value = default;
			if (_cursor <= 0) return false;

			_cursor--;
			value = _buffer[PhysicalIndex(_cursor)];
			return true;
		}

		internal bool TryGoForward(out T value)
		{
			value = default;
			if (_cursor >= _count - 1) return false;

			_cursor++;
			value = _buffer[PhysicalIndex(_cursor)];
			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < _count; i++)
			{
				yield return _buffer[PhysicalIndex(i)];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>) this).GetEnumerator();
		}
	}
}