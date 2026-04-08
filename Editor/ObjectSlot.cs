using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	[Serializable]
	internal class ObjectSlot : IBufferSlot
	{
		[SerializeField] public Object[] Value;
		
		[Serializable]
		internal class Converter : IBufferConverter<Object[], ObjectSlot>
		{
			public ObjectSlot ToBuffer(Object[] value) => new() { Value = value };
			public Object[] FromBuffer(ObjectSlot slot) => slot?.Value ?? Array.Empty<Object>();
		}
	}
}