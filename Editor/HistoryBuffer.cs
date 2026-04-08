using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Besttof.SelectionHistory
{
	[Serializable]
	internal class HistoryBuffer<T, TBuffer> : IHistoryBuffer<T>, ISerializationCallbackReceiver //where TBuffer : IBufferSlot
	{
		[SerializeField] private TBuffer[] _buffer;
		[SerializeReference] private IBufferConverter<T, TBuffer> _converter;

		[SerializeField] private int _head;
		[SerializeField] private int _count;
		[SerializeField] private int _cursor;

		public int Count => _count;
		public int Capacity => _buffer.Length;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int PhysicalIndex(int logical) => (_head + logical) % Capacity;

		internal HistoryBuffer(int capacity, IBufferConverter<T, TBuffer> converter)
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
			return GetEnumerator();
		}

		public void OnBeforeSerialize()
		{
			Debug.Log($"OnBeforeSerialize");
		}

		public void OnAfterDeserialize()
		{
			Debug.Log($"OnAfterDeserialize");
		}
	}
}