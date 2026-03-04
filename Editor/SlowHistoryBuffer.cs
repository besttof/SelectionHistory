using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Besttof.SelectionHistory
{
	/// <summary>
	/// This buffer uses the GlobalObjectID system to reliably keep track of objects that may be unloaded
	/// at some point in the history.
	/// </summary>
	internal class SlowHistoryBuffer : IHistoryBuffer<Object[]>
	{
		private readonly HistoryBuffer<GlobalObjectId[]> _innerBuffer;

		public SlowHistoryBuffer(int capacity)
		{
			_innerBuffer = new HistoryBuffer<GlobalObjectId[]>(capacity);
		}

		public IEnumerator<Object[]> GetEnumerator()
		{
			foreach (var ids in _innerBuffer)
			{
				var objects = new Object[ids.Length];
				GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(ids, objects);
				yield return objects;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => _innerBuffer.Count;
		public int Capacity => _innerBuffer.Capacity;

		public void Clear()
		{
			_innerBuffer.Clear();
		}

		public void Push(Object[] value)
		{
			var buffer = ObjectsToIdsBuffer(value);
			_innerBuffer.Push(buffer);
		}

		public bool TryGetCurrent(out Object[] value)
		{
			value = null;
			if (_innerBuffer.TryGetCurrent(out var ids))
			{
				IdsToObjectsBuffer(ids, out value);
				return true;
			}

			return false;
		}

		public bool TryGoBack(out Object[] value)
		{
			value = null;
			if (_innerBuffer.TryGoBack(out var ids))
			{
				Debug.Log("<- " + string.Join(", ", ids.Select(id => id.targetObjectId)));
				value = new Object[ids.Length];
				GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(ids, value);
				return true;
			}

			return false;
		}

		public bool TryGoForward(out Object[] value)
		{
			value = null;
			if (_innerBuffer.TryGoForward(out var ids))
			{
				Debug.Log("-> " + string.Join(", ", ids.Select(id => id.targetObjectId)));
				value = new Object[ids.Length];
				GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(ids, value);
				return true;
			}

			return false;
		}

		private static GlobalObjectId[] ObjectsToIdsBuffer(Object[] value)
		{
			var buffer = new GlobalObjectId[value.Length];
			GlobalObjectId.GetGlobalObjectIdsSlow(value, buffer);
			return buffer;
		}

		private static void IdsToObjectsBuffer(GlobalObjectId[] ids, out Object[] value)
		{
			value = new Object[ids.Length];
			GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(ids, value);
		}
	}
}