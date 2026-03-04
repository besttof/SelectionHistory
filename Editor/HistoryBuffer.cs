using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	internal static class HistoryBuffer
	{
		internal static IHistoryBuffer<Object[]> Create(int capacity, SelectionMode mode = SelectionMode.FastAndNaive)
		{
			return mode switch
			{
				SelectionMode.FastAndNaive   => new Buffer<Object[], Object[]>(capacity, new NoConverter<Object[]>()),
				SelectionMode.SlowAndCorrect => new Buffer<Object[], GlobalObjectId[]>(capacity, new GlobalIdConverter()),
				_                            => throw new ArgumentOutOfRangeException()
			};
		}

		internal static IHistoryBuffer<T> CreateRaw<T>(int capacity)
		{
			return new Buffer<T, T>(capacity, new NoConverter<T>());
		}

		private class Buffer<T, TBuffer> : IHistoryBuffer<T>
		{
			private readonly TBuffer[] _buffer;
			private readonly IBufferConverter<T, TBuffer> _converter;

			private int _head;
			private int _count;
			private int _cursor;

			public int Count => _count;
			public int Capacity => _buffer.Length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private int PhysicalIndex(int logical) => (_head + logical) % Capacity;

			internal Buffer(int capacity, IBufferConverter<T, TBuffer> converter)
			{
				_buffer = new TBuffer[capacity];
				_converter = converter;
				Clear();
			}

			public void Clear()
			{
				_head = 0;
				_count = 0;
				_cursor = -1;

				Array.Fill(_buffer, default);
			}

			public void Push(T value)
			{
				// Truncate the count to the cursor position
				_count = _cursor + 1;

				if (_count < Capacity)
				{
					_buffer[PhysicalIndex(_count)] = _converter.ToBuffer(value);
					_count++;
				}
				else
				{
					// When at capacity, overwrite the oldest value and move the head forward
					_buffer[PhysicalIndex(0)] = _converter.ToBuffer(value);
					_head = PhysicalIndex(1);
				}

				_cursor = _count - 1;
			}

			public bool TryGetCurrent(out T value)
			{
				value = default;
				if (_count == 0) return false;

				value = _converter.FromBuffer(_buffer[PhysicalIndex(_cursor)]);
				return true;
			}

			public bool TryGoBack(out T value)
			{
				value = default;
				if (_cursor <= 0) return false;

				_cursor--;
				value = _converter.FromBuffer(_buffer[PhysicalIndex(_cursor)]);
				return true;
			}

			public bool TryGoForward(out T value)
			{
				value = default;
				if (_cursor >= _count - 1) return false;

				_cursor++;
				value = _converter.FromBuffer(_buffer[PhysicalIndex(_cursor)]);
				return true;
			}

			public IEnumerator<T> GetEnumerator()
			{
				for (int i = 0; i < _count; i++)
				{
					yield return _converter.FromBuffer(_buffer[PhysicalIndex(i)]);
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<T>) this).GetEnumerator();
			}
		}
	}
}