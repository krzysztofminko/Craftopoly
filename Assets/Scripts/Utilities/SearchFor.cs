using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SearchFor
{
	//TODO: Limit search to single plot
	
	public static bool ItemInStorage(ItemType itemType, Storage storage, out Item item)
	{
		item = storage.items.Find(i => i.type == itemType);
		return item;
	}

	public static bool ItemInGatherStructures(ItemType itemType, Vector3 position, out Item item, out Storage storage, GatherStructure exclude = null)
	{
		GatherStructure gatherStructure = GatherStructure.list
							.FindAll(s => s != exclude && s.storage.Count(itemType) > 0)
							.OrderBy(s => (s.transform.position - position).magnitude)
							.FirstOrDefault();
		if (gatherStructure)
		{
			item = gatherStructure.storage.items.Find(i => i.type == itemType);
			storage = gatherStructure.storage;
			return true;
		}

		item = null;
		storage = null;
		return false;
	}

	public static bool ItemInCraftStructures(ItemType itemType, Vector3 position, out Item item, out Storage storage, CraftStructure exclude = null)
	{
		CraftStructure craftStructure = CraftStructure.list
			.FindAll(s => s != exclude && s.storage.Count(itemType) > 0)
			.OrderBy(s => (s.transform.position - position).sqrMagnitude)
			.FirstOrDefault();
		if (craftStructure)
		{
			item = craftStructure.storage.items.Find(i => i.type == itemType);
			storage = craftStructure.storage;
			return true;
		}

		item = null;
		storage = null;
		return false;
	}

	public static bool ItemInShopStructures(ItemType itemType, Vector3 position, out Item item, out Storage storage, ShopStructure exclude = null)
	{
		ShopStructure shop = ShopStructure.list
								.FindAll(s => s != exclude && s.storage.Count(itemType) > 0)
								.OrderBy(s => /*s.storage.items.Find(i => i.type == missing[0].type).value + */(s.transform.position - position).magnitude)
								.FirstOrDefault();
		if (shop)
		{
			item = shop.storage.items.Find(i => i.type == itemType);
			storage = shop.storage;
			return true;
		}

		item = null;
		storage = null;
		return false;
	}


	public static bool CraftStructureWithItemType(ItemType itemType, Vector3 position, out CraftStructure craftStructure, bool onlyAvailable = true)
	{
		craftStructure = CraftStructure.list
			.FindAll(s => s.itemTypes.Contains(itemType)) // && (onlyAvailable? s.currentItemType == itemType : true))
			.OrderBy(s => (s.transform.position - position).sqrMagnitude)
			.FirstOrDefault();
		return craftStructure;
	}


	public static bool FuelInStorage(Storage storage, out Item item)
	{
		item = storage.items.OrderByDescending(i => i.type.fuelValue).FirstOrDefault();
		return item;
	}

	public static bool FuelInGatherStructures(Vector3 position, out Item item, out Storage storage)
	{
		GatherStructure gatherStructure = GatherStructure.list
							.FindAll(s => s.storage.items.Find(i => i.type.fuelValue > 0))
							.OrderBy(s => (s.transform.position - position).magnitude)
							.FirstOrDefault();
		if (gatherStructure)
		{
			item = gatherStructure.storage.items.OrderByDescending(i => i.type.fuelValue).FirstOrDefault();
			storage = gatherStructure.storage;
			return true;
		}

		item = null;
		storage = null;
		return false;
	}

	public static bool FuelInCraftStructures(Vector3 position, out Item item)
	{
		CraftStructure craftStructure = CraftStructure.list
			.FindAll(s => s.craftedItem && s.craftedItem.type.fuelValue > 0)
			.OrderBy(s => (s.transform.position - position).sqrMagnitude)
			.FirstOrDefault();
		if (craftStructure)
		{
			item = craftStructure.craftedItem;
			return true;
		}

		item = null;
		return false;
	}

	public static bool FuelInShopStructures(Vector3 position, out Item item, out Storage storage)
	{
		ShopStructure shop = ShopStructure.list
								.FindAll(s => s.storage.items.Find(i => i.type.fuelValue > 0))
								.OrderBy(s => /*s.storage.items.Find(i => i.type == missing[0].type).value + */(s.transform.position - position).magnitude)
								.FirstOrDefault();
		if (shop)
		{
			item = shop.storage.items.OrderByDescending(i => i.type.fuelValue).FirstOrDefault();
			storage = shop.storage;
			return true;
		}

		item = null;
		storage = null;
		return false;
	}
}