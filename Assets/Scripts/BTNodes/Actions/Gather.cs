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

			source = _source.Value.GetComponent<Source>();
			timer = 0;
			citizen.animator.SetFloat("UseAnimationId", 1);
			source.ReservedBy = citizen;
			outItem.Value = null;
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