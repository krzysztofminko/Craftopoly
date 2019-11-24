using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Attack : CitizenAction
	{
		const float animationTimer = 0.2f;

		public SharedGameObject _targetHealth;

		private Health targetHealth;
		private float timer;

		public override void OnStart()
		{
			base.OnStart();

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