using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Idle : CitizenAction
	{
		public SharedFloat duration = 1;

		private float timer;

		public override void OnStart()
		{
			base.OnStart();

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