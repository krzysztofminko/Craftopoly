using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Craft : FsmStateAction
	{
		[CheckForComponent(typeof(CraftStructure))]
		[RequiredField]
		public FsmGameObject _craftStructure;
		[RequiredField]
		public FsmObject _itemType;
		public FsmGameObject returnItem;

		float timer;
		Citizen citizen;
		CraftStructure craftStructure;
		ItemType itemType;
		Target target;

		public override void OnExit()
		{
			timer = 0;
			target.ReservedBy = null;
			_craftStructure.Value = null;
			_itemType.Value = null;
		}

		public override void OnEnter()
		{
			if (!citizen)
				citizen = Owner.GetComponent<Citizen>();
			craftStructure = _craftStructure.Value.GetComponent<CraftStructure>();			
			itemType = (ItemType)_itemType.Value;
			target = craftStructure.target;
			target.ReservedBy = citizen;
		}

		public override void OnUpdate()
		{
			if (!_craftStructure.Value)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				Fsm.Event("FAILED");
			}
			if (citizen.skills.Get(itemType.requiredSkill.name) < itemType.requiredSkill.value)
			{
				if (citizen == Player.instance)
					Utilities.UI.Notifications.instance.Add(itemType.requiredSkill.name + " "+ itemType.requiredSkill.value + " required.");
				Fsm.Event("FAILED");
			}
			else
			{
				citizen.animator.SetFloat("UseAnimationId", 1);
				
				timer += Time.deltaTime;
				if (timer > itemType.blueprint.duration / Mathf.Max(0.1f, citizen.skills.Get(itemType.requiredSkill.name)))
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					if (itemType.blueprint.MissingResources(craftStructure.storage).Count > 0)
					{
						Fsm.Event("FAILED");
					}
					else
					{
						for (int i = 0; i < itemType.blueprint.requiredItems.Count; i++)
							craftStructure.storage.DestroyItem(itemType.blueprint.requiredItems[i].type, itemType.blueprint.requiredItems[i].count);

						Item crafted = itemType.Spawn(1, craftStructure.transform.position, craftStructure.transform.rotation);
						crafted.GetComponent<Rigidbody>().isKinematic = true;
						craftStructure.storage.AddItem(crafted);
						if(citizen == Player.instance)
							craftStructure.craftedItem = crafted;
						returnItem.Value = crafted.gameObject;

						CraftStructure.CraftOrder order = craftStructure.orders.Find(o => o.itemType == itemType);
						if (!order.maintainAmount)
							order.count = Mathf.Max(0, order.count - 1);

						craftStructure.SetCurrentItemBlueprint(null);

						Finish();
					}
				}
			}
		}


	}

}
