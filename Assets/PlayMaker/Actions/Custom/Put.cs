using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Put : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Storage))]
		public FsmGameObject _storage;
		[CheckForComponent(typeof(Item))]
		public FsmGameObject _stack;
		[Tooltip("If count = 0 then put all")]
		public FsmInt count;

		float timer;
		Citizen citizen;
		Storage storage;
		Item stack;
		Target goToTarget;

		public override void OnEnter()
		{
			citizen = Owner.GetComponent<Citizen>();

			storage = _storage.Value? _storage.Value.GetComponent<Storage>() : null;
			stack = _stack.Value ? _stack.Value.GetComponent<Item>() : null;
			goToTarget = storage ? storage.target : stack? stack.target : null;

			citizen.pickedItem.target.ReservedBy = citizen;

			if (count.Value < 1)
				count.Value = citizen.pickedItem.count;

			count.Value = Mathf.Clamp(count.Value, 1, citizen.pickedItem.count);
		}

		public override void OnExit()
		{
			timer = 0;
			_storage.Value = null;
			_stack.Value = null;
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
					citizen.pickedItem.target.ReservedBy = null;
										
					//TODO: Put count
					if (storage)
					{
						storage.AddItem(citizen.pickedItem, count.Value);
					}
					else if (stack)
					{
						stack.count += citizen.pickedItem.count;
						Object.Destroy(citizen.pickedItem.gameObject);
						Debug.Log(citizen.pickedItem + " destroyed", citizen.pickedItem);
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
