using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Release : CitizenAction
	{
		const float animationTimer = 0.2f;

		private float timer;

		public override void OnStart()
		{
			base.OnStart();
			timer = 0;
			citizen.animator.SetFloat("UseAnimationId", 1);
		}

		public override TaskStatus OnUpdate()
		{
			timer += Time.deltaTime;
			if (timer > animationTimer)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);

				citizen.pickedItem.ReservedBy = null;
				citizen.pickedItem.SetParent(null);
				citizen.pickedItem = null;

				return TaskStatus.Success;
			}
			return TaskStatus.Running;
		}
	}
}