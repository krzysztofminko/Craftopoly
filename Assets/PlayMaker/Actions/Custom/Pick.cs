using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Pick : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Storage))]
		public FsmGameObject _storage;
		[CheckForComponent(typeof(CraftStructure))]
		public FsmGameObject _craftStructure;
		[CheckForComponent(typeof(Item))]
		[RequiredField]
		public FsmGameObject _item;
		[Tooltip("If count = 0 then pick all")]
		public FsmInt count;
		public FsmBool updateItemReference = true;

		float timer;
		Citizen citizen;
		Item item;
		Storage storage;
		CraftStructure craftStructure;

		public override void OnEnter()
		{
			
			if(!citizen)
				citizen = Owner.GetComponent<Citizen>();
			item = _item.Value? _item.Value.GetComponent<Item>() : null;
			storage = _storage.Value ? _storage.Value.GetComponent<Storage>() : null;
			craftStructure = _craftStructure.Value ? _craftStructure.Value.GetComponent<CraftStructure>() : null;

			if (count.Value < 1)
				count.Value = item.count;

			count.Value = Mathf.Clamp(count.Value, 1, item.type.maxCount);
		}

		public override void OnExit()
		{
			timer = 0;
			count.Value = 0;
			_item.Value = null;
			_storage.Value = null;
		}

		public override void OnUpdate()
		{
			if (citizen.GoTo(item.target))
			{
				if (!item || (item && item.target.ReservedBy && item.target.ReservedBy != citizen))
				{
					citizen.animator.SetFloat("UseAnimationId", 0);
					Debug.Log("FAILED: item == null or ReservedBy", citizen);
					Fsm.Event("FAILED");
				}
				else if (storage && storage.target.shopStructure && storage.target.shopStructure.plot && citizen.Money < item.type.value * count.Value)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);
					Fsm.Event("FAILED");
				}
				else
				{
					item.target.ReservedBy = citizen;

					citizen.animator.SetFloat("UseAnimationId", 1);

					timer += Time.deltaTime;

					if (timer > animationTimer)
					{
						citizen.animator.SetFloat("UseAnimationId", 0);

						//item = citizen.Pick(item, count.Value, storage);

						if (storage)
						{
							citizen.pickedItem = storage.RemoveItem(item, count.Value);

							if (storage.target.shopStructure && storage.target.shopStructure.plot)
							{
								citizen.Pay(storage.target.shopStructure.plot, citizen.pickedItem.type.value * count.Value);
								storage.target.shopStructure.FinishTransaction();
							}
						}
						else if (craftStructure)
						{
							citizen.pickedItem = craftStructure.craftedItem;
							craftStructure.craftedItem = null;
						}
						else if (item.count - count.Value > 0)
						{
							item.count -= count.Value;
							item.target.ReservedBy = null;
							citizen.pickedItem = item.type.Spawn(count.Value, citizen.transform.position, citizen.transform.rotation);
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
