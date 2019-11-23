using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Pick : CitizenAction
	{
		const float animationTimer = 0.2f;

		public SharedGameObject _item;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Optional")]
		public SharedGameObject _storage;

		private Item item;
		private Storage storage;
		private float timer;

		public override void OnStart()
		{
			base.OnStart();

			item = _item.Value.GetComponent<Item>();
			storage = _storage.Value? _storage.Value.GetComponent<Storage>() : null;
			timer = 0;
			citizen.animator.SetFloat("UseAnimationId", 1);
		}

		public override TaskStatus OnUpdate()
		{
			if (!item || (item && item.ReservedBy && item.ReservedBy != citizen))
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskStatus.Failure;
			}
			else if (storage && storage.moneyReceiver != null && citizen.Money < item.type.value)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskStatus.Failure;
			}
			else
			{
				item.ReservedBy = citizen;

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					if (storage)
					{
						citizen.pickedItem = storage.RemoveItem(item);

						if (storage.moneyReceiver != null)
							citizen.Pay(storage.moneyReceiver, citizen.pickedItem.type.value);
					}
					else
					{
						citizen.pickedItem = item;
					}

					return TaskStatus.Success;
				}
			}
			return TaskStatus.Running;
		}
	}
}