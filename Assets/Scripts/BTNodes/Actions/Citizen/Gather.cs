using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Gather : CitizenAction
	{
		const float animationTimer = 0.2f;

		public SharedGameObject _source;
		public SharedGameObject outItem;

		private Source source;
		private float timer;

		public override void OnStart()
		{
			base.OnStart();

			source = _source.Value? _source.Value.GetComponent<Source>() : null;
			timer = 0;
			outItem.Value = null;
		}

		public override TaskStatus OnUpdate()
		{
			if (!source || (source.ReservedBy && source.ReservedBy != citizen))
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskStatus.Failure;
			}
			else if(citizen.GoTo(source.transform, 1))
			{
				citizen.animator.SetFloat("UseAnimationId", 1);
				source.ReservedBy = citizen;

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					Item item = source.Gather(20 * Mathf.Max(0.1f, citizen.skills.Get(source.itemType.requiredSkill.name)));
					if (item)
					{
						outItem.Value = item.gameObject;
						source.ReservedBy = null;
						return TaskStatus.Success;
					}
				}
			}
			return TaskStatus.Running;
		}
	}
}