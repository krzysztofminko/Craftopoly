using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes
{
	public abstract class CitizenAction : Action
	{
		protected Citizen citizen;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			if (!citizen)
				Debug.LogError($"Owner of the node has no {typeof(Citizen).Name} component.", gameObject);
		}
	}
}