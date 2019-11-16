using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Storage))]
public class CraftStructure : Workplace
{
	public static new List<CraftStructure> list = new List<CraftStructure>();

	[System.Serializable]
	public class CraftOrder
	{
		public ItemType itemType;
		public int count;
		public bool maintainAmount;

		public CraftOrder(ItemType itemType, int count, bool decreaseCount)
		{
			this.itemType = itemType;
			this.count = count;
			this.maintainAmount = decreaseCount;
		}
	}

	[Header("Settings")]
	[Header("CraftStructure")]
	public List<ItemType> itemTypes = new List<ItemType>();
	[Tooltip("If 0 - require worker activity to craft")]
	public float fuelMax;
	[SerializeField]
	private int GetItemCalls;
	[SerializeField]
	private int GetItemCallsMax = 10;

	[Header("References")]
	[SerializeField] private Transform craftedTransform;
	[SerializeField] private ParticleSystem smokeParticles;
	[SerializeField] private ParticleSystem fireParticles;

	[Header("Runtime")]
	public float fuel;
	public float progress;
	private bool resourcesUsed;
	[SerializeField]
	private ItemType _currentItemType;
	public ItemType currentItemType
	{
		get => _currentItemType;
		set
		{
			if (_currentItemType && _currentItemType != value && value)
				ReleaseUnnecessaryItems(value);
			_currentItemType = value;
		}
	}
	[SerializeField]
	private Item _craftedItem;
	public Item craftedItem
	{
		get => _craftedItem;
		set
		{
			_craftedItem = value;
			_craftedItem?.SetParent(craftedTransform ? craftedTransform : transform);
		}
	}
	public List<CraftOrder> orders;

	private bool isFueled;

	protected override void Awake()
	{
		base.Awake();
		list.Add(this);
		isFueled = fuelMax > 0;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		list.Remove(this);
	}

	private void Start()
	{
		for (int i = 0; i < itemTypes.Count; i++)
			orders.Add(new CraftOrder(itemTypes[i], 0, true));
	}

	private void _Update()
    {
		if (isFueled)
		{
			Refuel();
			Burn();
		}

		if (craftedItem && (craftedItem.transform.parent != transform && craftedItem.transform.parent != craftedTransform))
			craftedItem = null;

		if (ReservedBy != Player.instance && worker && worker.WorkTime() && worker.fsm.ActiveStateName == "Idle")
		{
			//Store craftedItem
			if (craftedItem)
			{
				if (SearchFor.NearestStorageStructure(plot, transform.position, out StorageStructure targetStorage))
					worker.fsm.Store(craftedItem, null, targetStorage.storage);
				else
					Debug.LogError("No StorageStructure on plot");
			}
			else
			{
				//Find first uncomplete order
				CraftOrder order = orders.Find(o => (!o.maintainAmount && o.count > 0) || (o.maintainAmount && o.count > storage.Count(o.itemType)));
				if (order != null)
				{
					currentItemType = order.itemType;

					//Find what is missing
					List<ItemCount> missing = order.itemType.blueprint.MissingResources(storage);
					if (missing.Count > 0)
					{
						worker.GetItems(order.itemType.blueprint.requiredItems, storage);
					}
					else
					{
						if (isFueled)
						{
							//Refuel
							float missingFuel = order.itemType.blueprint.duration - fuel;
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
									worker.fsm.Store(fuelItem, sourceStorage, storage);
							}
						}
						else
						{
							//Craft
							worker.fsm.Craft(order.itemType, this);
						}
					}
				}
			}
		}
    }


	private void Burn()
	{
		//Burn
		fuel = Mathf.Max(0, fuel - Time.deltaTime);
		if (fuel > 0)
		{
			if (!smokeParticles.isPlaying)
				smokeParticles.Play();
			if (!fireParticles.isPlaying)
				fireParticles.Play();

			if (!craftedItem && currentItemType)
			{
				//Use resources
				if (!resourcesUsed)
				{
					if (currentItemType.blueprint.MissingResources(storage).Count == 0)
					{
						for (int i = 0; i < currentItemType.blueprint.requiredItems.Count; i++)
							storage.DestroyItemType(currentItemType.blueprint.requiredItems[i].type, currentItemType.blueprint.requiredItems[i].count);
						resourcesUsed = true;
					}
				}
				else
				{
					progress += Time.deltaTime / currentItemType.blueprint.duration;
					if (progress >= 1)
					{
						//Craft
						progress = 0;
						resourcesUsed = false;
						craftedItem = currentItemType.Spawn(1, transform.position, transform.rotation);
						craftedItem.GetComponent<Rigidbody>().isKinematic = true;
						currentItemType = null;
					}
				}
			}
		}
		else
		{
			if (smokeParticles.isPlaying)
				smokeParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
			if (fireParticles.isPlaying)
				fireParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	private void Refuel()
	{
		List<Item> fuelItems = storage.items.FindAll(i => !i.ReservedBy && i.type.fuelValue > 0).ToList();
		for(int i = fuelItems.Count - 1; i >= 0; i --)
		{
			fuel = Mathf.Min(fuelMax, fuel + fuelItems[i].type.fuelValue);
			storage.DestroyItemType(fuelItems[i].type, 1);
		}
	}

	private void ReleaseUnnecessaryItems(ItemType type)
	{
		for (int i = storage.items.Count - 1; i >= 0; i--)
			if (type.blueprint.requiredItems.FindAll(r => r.type == storage.items[i].type).Count == 0 || (craftedItem && craftedItem != storage.items[i].type))
			{
				Debug.Log(storage.items[i].type);
				Debug.Log(type.blueprint.requiredItems.Count);
				Debug.Log(type.blueprint.requiredItems.FindAll(r => r.type == storage.items[i].type).Count);
				if (storage.items[i] == craftedItem)
					craftedItem = null;
				storage.RemoveItem(storage.items[i]);
				
			}
	}

}
