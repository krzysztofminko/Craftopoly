using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Put : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Storage))]
		public FsmGameObject _storage;

		float timer;
		Citizen citizen;
		Storage storage;
		Transform goToTarget;

		public override void OnEnter()
		{
			citizen = Owner.GetComponent<Citizen>();

			storage = _storage.Value? _storage.Value.GetComponent<Storage>() : null;
			goToTarget = storage ? storage.transform : null;

			citizen.pickedItem.ReservedBy = citizen;

		}

		public override void OnExit()
		{
			timer = 0;
			_storage.Value = null;
		}

		public override void OnUpdate()
		{
			if (!goToTarget || citizen.GoTo(goToTarget))
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

					Finish();
				}
			}
		}


	}

}
