using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Put : CitizenAction
	{
		const float animationTimer = 0.2f;

		public SharedGameObject _storage;

		private Storage storage;
		private float timer;

		public override void OnStart()
		{
			base.OnStart();

			storage = _storage.Value.GetComponent<Storage>();
			timer = 0;
			citizen.animator.SetFloat("UseAnimationId", 1);
		}

		public override TaskStatus OnUpdate()
		{
			if (!storage)
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

					citizen.pickedItem.ReservedBy = null;
					storage.AddItem(citizen.pickedItem);
					citizen.pickedItem = null;

					return TaskStatus.Success;
				}
			}
			return TaskStatus.Running;
		}
	}
}