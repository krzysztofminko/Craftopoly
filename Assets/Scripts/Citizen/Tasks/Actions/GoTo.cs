using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks.Actions
{
	[TaskCategory("Citizen")]
	public class GoTo : Action
	{
		public SharedGameObject target;
		public SharedVector3 position;
		public SharedFloat proximity;
		public bool reserveTarget;

		private Citizen citizen;
		private IReserve reserve;
		private bool goToTarget;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			goToTarget = target.Value;
			if (goToTarget && reserveTarget)
			{
				reserve = target.Value.GetComponent<IReserve>();
				reserve.ReservedBy = citizen;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (goToTarget && !target.Value)
				return TaskStatus.Failure;

			if(citizen.GoTo(target.Value ? target.Value.transform.position : position.Value, proximity.Value))
			{
				if (reserveTarget)
					reserve.ReservedBy = null;
				return TaskStatus.Success;
			}

			return  TaskStatus.Running;
		}
	}
}