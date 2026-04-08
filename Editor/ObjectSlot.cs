using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	[Serializable]
	internal struct ObjectSlot : IBufferSlot<Object[]>
	{
		[SerializeField] public Object[] Value;

		public void Set(Object[] value) => Value = value;
		public Object[] Get() => Value ?? Array.Empty<Object>();
	}
}