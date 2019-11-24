using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes
{

	public abstract class CitizenConditional : Conditional
	{
		protected Citizen citizen;

		public override TaskStatus OnUpdate()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			if (!citizen)
				Debug.LogError($"Owner of the node has no {typeof(Citizen).Name} component.", gameObject);

			return TaskStatus.Success;
		}
	}
}