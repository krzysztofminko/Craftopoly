using UnityEngine;
using BehaviorDesigner.Runtime;

namespace BTVariables
{
	[System.Serializable]
	public class SharedCitizen : SharedVariable<Citizen>
	{
		public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
		public static implicit operator SharedCitizen(Citizen value) { return new SharedCitizen { mValue = value }; }
	}
}