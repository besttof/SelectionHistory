using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	[Serializable]
	internal struct GlobalObjectIdSlot : IBufferSlot<Object[]>
	{
		[SerializeField] public GlobalObjectId[] Value;

		public void Set(Object[] value)
		{
			var ids = new GlobalObjectId[value.Length];
			GlobalObjectId.GetGlobalObjectIdsSlow(value, ids);
			Value = ids;
		}

		public Object[] Get()
		{
			var ids = Value ?? Array.Empty<GlobalObjectId>();
			var objects = new Object[ids.Length];
			GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(ids, objects);
			return objects;
		}
	}
}