using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Craft : CitizenAction
	{
		public SharedGameObject _craftStructure;
		public SharedObject _itemType;
		public SharedGameObject returnItem;

		private CraftStructure craftStructure;
		private ItemType itemType;
		private float timer;

		public override void OnStart()
		{
			base.OnStart();

			craftStructure = _craftStructure.Value.GetComponent<CraftStructure>();
			itemType = (ItemType)_itemType.Value;
			timer = 0;
			returnItem = null;
		}

		public override TaskStatus OnUpdate()
		{
			if (!craftStructure)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskStatus.Failure;
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
						return TaskStatus.Failure;
					}
					else
					{
						for (int i = 0; i < itemType.blueprint.requiredItems.Count; i++)
							craftStructure.storage.DestroyItemType(itemType.blueprint.requiredItems[i].type, itemType.blueprint.requiredItems[i].count);

						Item crafted = itemType.Spawn(1, craftStructure.transform.position, craftStructure.transform.rotation);
						craftStructure.craftedItem = crafted;
						returnItem.Value = crafted.gameObject;

						CraftStructure.CraftOrder order = craftStructure.orders.Find(o => o.itemType == itemType);
						if (!order.maintainAmount)
							order.count = Mathf.Max(0, order.count - 1);

						return TaskStatus.Success;
					}
				}
			}
			return TaskStatus.Running;
		}
	}
}