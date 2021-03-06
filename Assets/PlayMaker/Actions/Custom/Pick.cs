using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	
	[ActionCategory("Custom")]
	public class Pick : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Storage))]
		public FsmGameObject _storage;
		[CheckForComponent(typeof(Item))]
		[RequiredField]
		public FsmGameObject _item;
		public FsmBool updateItemReference = true;

		float timer;
		Citizen citizen;
		Item item;
		Storage storage;

		public override void OnEnter()
		{
			
			if(!citizen)
				citizen = Owner.GetComponent<Citizen>();
			item = _item.Value? _item.Value.GetComponent<Item>() : null;
			storage = _storage.Value ? _storage.Value.GetComponent<Storage>() : null;
		}

		public override void OnExit()
		{
			timer = 0;
			_item.Value = null;
			_storage.Value = null;
		}

		public override void OnUpdate()
		{
			if (!item || (item && item.ReservedBy && item.ReservedBy != citizen))
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				Fsm.Event("FAILED");
			}
			else if (citizen.GoTo(item.transform))
			{
				
				if (storage && storage.moneyReceiver != null && citizen.Money < item.type.value)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);
					Fsm.Event("FAILED");
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

						if (updateItemReference.Value)
							_item.Value = citizen.pickedItem.gameObject;

						Finish();
					}
				}
			}
		}


	}

}
