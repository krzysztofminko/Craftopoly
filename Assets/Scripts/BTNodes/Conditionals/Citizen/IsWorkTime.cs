using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Conditionals
{
	[TaskCategory("Citizen")]
	public class IsWorkTime : CitizenConditional
	{
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();

			if (!citizen.workplace)
				return TaskStatus.Failure;
			return citizen.WorkTime() ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}