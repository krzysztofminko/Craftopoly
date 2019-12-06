using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public class Put : CitizenTask
	{
		private const float animationTimer = 0.2f;

		private Storage storage;

		private float timer;

		public Put(Storage storage = null)
		{
			this.storage = storage;
		}

		protected override void Start()
		{
			base.Start();

			target = citizen.pickedItem.transform;

			timer = 0;
		}

		public override string ToString()
		{
			return $"{base.ToString()}(to {(storage ? storage.name : "ground")})";
		}

		protected override TaskState Execute()
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

					return TaskState.Success;
				}
			}

			return TaskState.Running;
		}
	}
}