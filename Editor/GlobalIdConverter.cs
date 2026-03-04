using UnityEditor;
using UnityEngine;

namespace Besttof.SelectionHistory
{
	internal class GlobalIdConverter : IBufferConverter<Object[], GlobalObjectId[]>
	{
		public GlobalObjectId[] ToBuffer(Object[] value)
		{
			var ids = new GlobalObjectId[value.Length];
			GlobalObjectId.GetGlobalObjectIdsSlow(value, ids);
			return ids;
		}

		public Object[] FromBuffer(GlobalObjectId[] bufferValue)
		{
			var objects = new Object[bufferValue.Length];
			GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(bufferValue, objects);
			return objects;
		}
	}
}