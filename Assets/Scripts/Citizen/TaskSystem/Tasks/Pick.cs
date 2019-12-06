using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public class Pick : CitizenTask
	{
		private const float animationTimer = 0.2f;

		private Item item;
		private Storage storage;

		private float timer;

		public Pick(Item item, Storage storage = null)
		{
			this.item = item;
			this.storage = storage;
			target = item.transform;
		}

		public override string ToString()
		{
			return $"{base.ToString()}({(item ? item.name : string.Empty)}, from {(storage ? storage.name : "ground")})";
		}

		protected override void Start()
		{
			base.Start();

			timer = 0;

			if (citizen.pickedItem)
				Debug.LogError("Citizen has pickedItem before. Can't pick new one.", citizen);
		}

		protected override TaskState Execute()
		{
			if (!item || (item && item.ReservedBy && item.ReservedBy != citizen))
			{
				if (!item)
					receiver.Log(this, "item == null");
				else if ((item && item.ReservedBy && item.ReservedBy != citizen))
					receiver.Log(this, $"Item is reserved by {item.ReservedBy.name}.");

				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskState.Failure;
			}

			if (citizen.GoTo(item.transform))
			{
				if (storage && storage.moneyReceiver != null && citizen.Money < item.type.value)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);
					return TaskState.Failure;
				}
				else
				{
					item.ReservedBy = citizen;

					citizen.animator.SetFloat("UseAnimationId", 1);

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

						return TaskState.Success;
					}
				}
			}

			return TaskState.Running;
		}
	}
}