using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class DetachTool : CitizenAction
	{
		const float animationTimer = 0.2f;

		private float timer;

		public override void OnStart()
		{
			base.OnStart();

			timer = 0;
		}

		public override TaskStatus OnUpdate()
		{
			citizen.animator.SetFloat("UseAnimationId", 1);

			timer += Time.deltaTime;
			if (timer > animationTimer)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);

				Item tmpTool = null;
				if (citizen.attachedTool)
					tmpTool = citizen.attachedTool;

				citizen.pickedItem = citizen.attachedTool;
				citizen.attachedTool = null;

				return TaskStatus.Success;
			}
			return TaskStatus.Running;
		}
	}
}