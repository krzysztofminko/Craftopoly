using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks.Actions
{
	[TaskCategory("Citizen")]
	public class Attack : Action
	{
		const float animationTimer = 0.2f;

		public SharedGameObject _targetHealth;

		private Citizen citizen;
		private Health targetHealth;
		private float timer;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			targetHealth = _targetHealth.Value.GetComponent<Health>();
		}

		public override TaskStatus OnUpdate()
		{
			if (!targetHealth)
			{
				return TaskStatus.Failure;
			}
			else
			{
				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					return targetHealth.Damage(20 * Mathf.Max(0.1f, citizen.skills.Get(Skills.Name.Fight))) ? TaskStatus.Success : TaskStatus.Failure;
				}
			}
			return TaskStatus.Running;
		}
	}
}