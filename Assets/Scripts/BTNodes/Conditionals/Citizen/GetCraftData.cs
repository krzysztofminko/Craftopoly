using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using BTVariables;

namespace BTNodes.Conditionals
{
	[TaskCategory("Citizen")]
	public class GetCraftData : CitizenConditional
	{
		public SharedGameObject outCraftedItem;
		public SharedGameObject outTargetStorage;
		public SharedItemType outMissingItemType;
		public SharedInt outMissingCount;
		public SharedFloat outMissingFuel;
		public SharedItemType outItemTypeToCraft;

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();

			outCraftedItem.Value = null;
			outTargetStorage.Value = null;
			outMissingItemType.Value = null;
			outMissingCount.Value = 0;
			outMissingFuel.Value = 0;
			outItemTypeToCraft.Value = null;

			CraftStructure craftStructure = citizen.workplace.GetComponent<CraftStructure>();
			if (!craftStructure)
				return TaskStatus.Failure;

			//Store craftedItem
			if (craftStructure.craftedItem)
			{
				outCraftedItem.Value = craftStructure.craftedItem.gameObject;
				if (SearchFor.NearestStorageStructure(craftStructure.plot, craftStructure.transform.position, out StorageStructure targetStorage))
				{
					outTargetStorage.Value = targetStorage.gameObject;
					return TaskStatus.Success;
				}
				else
				{
					Debug.LogError("No StorageStructure on plot", craftStructure.plot);
					return TaskStatus.Failure;
				}
			}
			else
			{
				//Find first uncomplete order
				CraftStructure.CraftOrder order = craftStructure.GetOrder();
				if (order != null)
				{
					//craftStructure.currentItemType = order.itemType;

					//Find what is missing
					List<ItemCount> missing = order.itemType.blueprint.MissingResources(craftStructure.storage);
					if (missing.Count > 0)
					{
						outMissingItemType.Value = missing[0].type;
						outMissingCount.Value = missing[0].count;
						return TaskStatus.Success;
					}
					else
					{
						if (craftStructure.IsFueled)
						{
							//Refuel
							outMissingFuel.Value = order.itemType.blueprint.duration - craftStructure.fuel;
							if (outMissingFuel.Value > 0)
								return TaskStatus.Success;
							/*
							if (missingFuel > 0)
							{
								Item fuelItem = null;
								Storage sourceStorage = null;

								if (!fuelItem)
									SearchFor.FuelInStorageStructures(transform.position, out fuelItem, out sourceStorage);
								if (!fuelItem)
									SearchFor.FuelInCraftStructures(transform.position, out fuelItem);
								if (!fuelItem)
									SearchFor.FuelInShopStructures(transform.position, out fuelItem, out sourceStorage);

								if (fuelItem)
									citizen.fsm.Store(fuelItem, sourceStorage, craftStructure.storage);
							}
							*/
						}
						else
						{
							//Craft
							outItemTypeToCraft.Value = order.itemType;
							return TaskStatus.Success;
						}
					}
				}
			}
			
			return TaskStatus.Success;
		}
	}
}