using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks
{
	[TaskCategory("Citizen")]
	public class Put : Action
	{
		const float animationTimer = 0.2f;

		public SharedGameObject _storage;

		private Citizen citizen;
		private Storage storage;
		private float timer;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			storage = _storage.Value ? _storage.Value.GetComponent<Storage>() : null;
			timer = 0;
		}

		public override TaskStatus OnUpdate()
		{
			if (!storage || citizen.GoTo(storage.transform))
			{
				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);
					citizen.pickedItem.ReservedBy = null;

					if (storage)
					{
						storage.AddItem(citizen.pickedItem);
					}
					else
					{
						citizen.pickedItem.SetParent(null);
					}

					citizen.pickedItem = null;

					return TaskStatus.Success;
				}
			}
			return TaskStatus.Running;
		}
	}
}