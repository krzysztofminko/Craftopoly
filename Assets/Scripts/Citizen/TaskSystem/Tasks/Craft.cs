using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public class Craft : CitizenTask
	{
		private ItemType itemType;
		private CraftStructure craftStructure;

		public Item outItem;

		private float timer;

		public Craft(CraftStructure craftStructure, ItemType itemType)
		{
			target = craftStructure.transform;

			this.craftStructure = craftStructure;
			this.itemType = itemType;
		}

		public override string ToString()
		{
			return $"{base.ToString()}({itemType.name} in {craftStructure.name})";
		}

		protected override void Start()
		{
			base.Start();
			
		}

		protected override TaskState Execute()
		{
			if (!craftStructure)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskState.Failure;
			}
			/*if (citizen.skills.Get(itemType.requiredSkill.name) < itemType.requiredSkill.value)
			{
				if (citizen == Player.instance)
					Utilities.UI.Notifications.instance.Add(itemType.requiredSkill.name + " " + itemType.requiredSkill.value + " required.");
				return TaskState.Failure;
			}*/
			
			citizen.animator.SetFloat("UseAnimationId", 1);

			timer += Time.deltaTime;
			if (timer > itemType.blueprint.duration / Mathf.Max(0.1f, citizen.skills.Get(itemType.requiredSkill.name)))
			{
				citizen.animator.SetFloat("UseAnimationId", 0);

				if (itemType.blueprint.MissingResources(craftStructure.storage).Count > 0)
				{
					return TaskState.Failure;
				}
				else
				{
					for (int i = 0; i < itemType.blueprint.requiredItems.Count; i++)
						craftStructure.storage.DespawnItemType(itemType.blueprint.requiredItems[i].type, itemType.blueprint.requiredItems[i].count);

					Item crafted = itemType.Spawn(1, craftStructure.transform.position, craftStructure.transform.rotation);
					craftStructure.CraftedItem = crafted;
					outItem = crafted;

					CraftStructure.CraftOrder order = craftStructure.orders.Find(o => o.itemType == itemType);
					if (!order.maintainAmount)
						order.count = Mathf.Max(0, order.count - 1);

					//craftStructure.SetCurrentItemBlueprint(null);

					return TaskState.Success;
				}
			}

			return TaskState.Running;
		}
	}
}