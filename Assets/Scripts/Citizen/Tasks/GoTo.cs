using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks
{
	[TaskCategory("Citizen")]
	public class GoTo : Action
	{
		public SharedGameObject target;
		public SharedVector3 position;
		public SharedFloat proximity;

		private Citizen citizen;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
		}

		public override TaskStatus OnUpdate()
		{
			return citizen.GoTo(target.Value ? target.Value.transform.position : position.Value, proximity.Value) ? TaskStatus.Success : TaskStatus.Running;
		}
	}
}