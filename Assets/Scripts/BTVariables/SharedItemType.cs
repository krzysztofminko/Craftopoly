using UnityEngine;
using BehaviorDesigner.Runtime;

namespace BTVariables
{
	[System.Serializable]
	public class SharedItemType : SharedVariable<ItemType>
	{
		public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
		public static implicit operator SharedItemType(ItemType value) { return new SharedItemType { mValue = value }; }
	}
}