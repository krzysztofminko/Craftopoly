using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks.Actions
{
	[TaskCategory("Citizen")]
	public class Idle : Action
	{
		public SharedFloat duration = 1;

		private Citizen citizen;
		private float timer;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			citizen.animator.SetBool("Walk", false);
			citizen.animator.SetFloat("UseAnimationId", 0);
			timer = 0;
		}

		public override TaskStatus OnUpdate()
		{
			timer += Time.deltaTime;
			return timer > duration.Value ? TaskStatus.Success : TaskStatus.Running;
		}
	}
}