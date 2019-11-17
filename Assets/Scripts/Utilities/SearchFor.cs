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

	public static bool ItemInStorageStructures(ItemType itemType, Vector3 position, out Item item, out Storage storage, StorageStructure exclude = null)
	{
		StorageStructure storageStructure = StorageStructure.list
							.FindAll(s => s != exclude && s.storage.Count(itemType) > 0)
							.OrderBy(s => (s.transform.position - position).magnitude)
							.FirstOrDefault();
		if (storageStructure)
		{
			item = storageStructure.storage.items.Find(i => i.type == itemType);
			storage = storageStructure.storage;
			return true;
		}

		item = null;
		storage = null;
		return false;
	}

	public static bool ItemInCraftStructures(ItemType itemType, Vector3 position, out Item item, CraftStructure exclude = null)
	{
		CraftStructure craftStructure = CraftStructure.list
			.FindAll(s => s != exclude && s.craftedItem && s.craftedItem.type == itemType)
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


	public static bool CraftStructureWithItemType(ItemType itemType, Vector3 position, out CraftStructure craftStructure, bool onlyWithoutWorker = true)
	{
		craftStructure = CraftStructure.list
			.FindAll(s => s.itemTypes.Contains(itemType) && (onlyWithoutWorker? !s.worker : true))
			.OrderBy(s => (s.transform.position - position).sqrMagnitude)
			.FirstOrDefault();
		return craftStructure;
	}

	public static bool GatherStructureWithItemType(ItemType itemType, Vector3 position, out GatherStructure gatherStructure)
	{
		gatherStructure = GatherStructure.list
			.FindAll(s => s.itemType == itemType)
			.OrderBy(s => (s.transform.position - position).sqrMagnitude)
			.FirstOrDefault();
		return gatherStructure;
	}


	public static bool FuelInStorage(Storage storage, out Item item)
	{
		item = storage.items.OrderByDescending(i => i.type.fuelValue).FirstOrDefault();
		return item;
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
								.OrderBy(s => (s.transform.position - position).magnitude)
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

	public static bool FuelInStorageStructures(Vector3 position, out Item item, out Storage storage)
	{
		StorageStructure storageStructure = StorageStructure.list
								.FindAll(s => s.storage.items.Find(i => i.type.fuelValue > 0))
								.OrderBy(s => (s.transform.position - position).magnitude)
								.FirstOrDefault();
		if (storageStructure)
		{
			item = storageStructure.storage.items.OrderByDescending(i => i.type.fuelValue).FirstOrDefault();
			storage = storageStructure.storage;
			return true;
		}

		item = null;
		storage = null;
		return false;
	}


	public static bool NearestStorageStructure(Plot plot, Vector3 position, out StorageStructure storageStructure)
	{
		storageStructure = plot.storageStructures.OrderBy(s => (s.transform.position - position).magnitude).FirstOrDefault();
		return storageStructure;
	}

	public static bool NearestStorageStructure(Plot plot, Vector3 position, out GameObject storageStructure)
	{
		StorageStructure tmp;
		if(plot)
			tmp = plot.storageStructures.OrderBy(s => (s.transform.position - position).magnitude).FirstOrDefault();
		else
			tmp = StorageStructure.list.OrderBy(s => (s.transform.position - position).magnitude).FirstOrDefault();

		storageStructure = tmp ? tmp.gameObject : null;

		return tmp;
	}

	public static bool ItemOnTheGround(ItemType itemType, Plot plot, Vector3 position, out GameObject item)
	{
		Item i;
		if(plot)
			//TODO: implement plot ground searching
			i = Item.free.FindAll(r => r.type == itemType && !r.ReservedBy).OrderBy(r => Distance.Manhattan2D(position, r.transform.position)).FirstOrDefault();
		else
			i = Item.free.FindAll(r => r.type == itemType && !r.ReservedBy).OrderBy(r => Distance.Manhattan2D(position, r.transform.position)).FirstOrDefault();

		item = i ? i.gameObject : null;

		return i;
	}
}