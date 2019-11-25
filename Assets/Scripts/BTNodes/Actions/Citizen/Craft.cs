using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BTVariables;

namespace BTNodes.Actions
{
	[TaskCategory("Citizen")]
	public class Craft : CitizenAction
	{
		public SharedGameObject _craftStructure;
		public SharedItemType itemType;
		public SharedGameObject returnItem;

		private CraftStructure craftStructure;
		private float timer;

		public override void OnStart()
		{
			base.OnStart();

			craftStructure = _craftStructure.Value.GetComponent<CraftStructure>();
			timer = 0;
			returnItem = null;
		}

		public override TaskStatus OnUpdate()
		{
			if (!craftStructure || !itemType.Value)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskStatus.Failure;
			}
			else
			{
				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > itemType.Value.blueprint.duration / Mathf.Max(0.1f, citizen.skills.Get(itemType.Value.requiredSkill.name)))
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					if (itemType.Value.blueprint.MissingResources(craftStructure.storage).Count > 0)
					{
						return TaskStatus.Failure;
					}
					else
					{
						for (int i = 0; i < itemType.Value.blueprint.requiredItems.Count; i++)
							craftStructure.storage.DestroyItemType(itemType.Value.blueprint.requiredItems[i].type, itemType.Value.blueprint.requiredItems[i].count);

						Item crafted = itemType.Value.Spawn(1, craftStructure.transform.position, craftStructure.transform.rotation);
						craftStructure.craftedItem = crafted;
						if (returnItem != null)
							returnItem.Value = crafted.gameObject;

						CraftStructure.CraftOrder order = craftStructure.orders.Find(o => o.itemType == itemType.Value);
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