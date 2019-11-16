using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks.Actions
{
	[TaskCategory("Citizen")]
	public class Release : Action
	{
		const float animationTimer = 0.2f;


		private Citizen citizen;
		private float timer;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
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