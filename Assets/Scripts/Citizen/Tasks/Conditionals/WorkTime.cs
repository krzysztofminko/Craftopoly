using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks.Conditionals
{
	[TaskCategory("Citizen")]
	public class WorkTime : Conditional
	{
		private Citizen citizen;

		public override TaskStatus OnUpdate()
		{
			if(!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			if (!citizen.workplace)
				return TaskStatus.Failure;
			return citizen.WorkTime() ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}