using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public class CraftJob : CitizenTask
	{
		private CraftStructure craftStructure;
		private ItemType itemType;
		private int count;
		private bool maintainAmount;

		public CraftJob(CraftStructure craftStructure, ItemType itemType)
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
			if (craftStructure.CraftedItem)
			{ 
				receiver.Receive(new Store(craftStructure.CraftedItem, craftStructure.targetStorage.storage));
			}
			else
			{
				if (count == 0)
					return TaskState.Success;

				List<ItemCount> missing = itemType.blueprint.MissingResources(craftStructure.storage);
				if (missing.Count > 0)
				{
					if (!citizen.GetItemsV2(missing, craftStructure.storage, out Storage outStorage, out Item outItem))
						return TaskState.Failure;
					else
						receiver.Receive(new Store(outItem, craftStructure.storage, outStorage));
				}
				else
				{
					receiver.Receive(new Craft(craftStructure, itemType));
				}
			}
			return TaskState.Running;
		}
	}
}