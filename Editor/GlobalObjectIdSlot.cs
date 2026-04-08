using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	[Serializable]
	internal class GlobalObjectIdSlot : IBufferSlot
	{
		[SerializeField] public GlobalObjectId[] Value;

		[Serializable]
		internal class Converter : IBufferConverter<Object[], GlobalObjectIdSlot>
		{
			public GlobalObjectIdSlot ToBuffer(Object[] value)
			{
				var ids = new GlobalObjectId[value.Length];
				GlobalObjectId.GetGlobalObjectIdsSlow(value, ids);
				return new GlobalObjectIdSlot { Value = ids };
			}

			public Object[] FromBuffer(GlobalObjectIdSlot slot)
			{
				var ids = slot?.Value ?? Array.Empty<GlobalObjectId>();
				var objects = new Object[ids.Length];
				GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(ids, objects);
				return objects;
			}
		}
	}
}