using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class GoTo : CitizenAction
	{
		public SharedGameObject target;
		public SharedVector3 position;
		public SharedFloat proximity = 1;
		public bool reserveTarget;
		public bool failureIfReserved;

		private IReserve reserve;
		private bool goToTarget;

		public override void OnStart()
		{
			base.OnStart();

			goToTarget = target.Value;
			if (failureIfReserved || reserveTarget)
				reserve = target.Value.GetComponent<IReserve>();
			if (goToTarget && reserveTarget)
				reserve.ReservedBy = citizen;
		}

		public override TaskStatus OnUpdate()
		{
			if (goToTarget && !target.Value)
				return TaskStatus.Failure;

			if (failureIfReserved && reserve.ReservedBy && reserve.ReservedBy != citizen)
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