using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks.Actions
{
	[TaskCategory("Citizen")]
	public class Gather : Action
	{
		const float animationTimer = 0.2f;

		public SharedGameObject _source;
		public SharedGameObject returnItem;

		private Citizen citizen;
		private Source source;
		private float timer;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			source = _source.Value.GetComponent<Source>();
			timer = 0;
			citizen.animator.SetFloat("UseAnimationId", 1);
			source.ReservedBy = citizen;
		}

		public override TaskStatus OnUpdate()
		{
			if (!source)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskStatus.Failure;
			}
			else
			{

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					Item item = source.Gather(20 * Mathf.Max(0.1f, citizen.skills.Get(source.itemType.requiredSkill.name)));
					if (item)
					{
						returnItem.Value = item.gameObject;
						source.ReservedBy = null;
						return TaskStatus.Success;
					}
				}
			}
			return TaskStatus.Running;
		}
	}
}