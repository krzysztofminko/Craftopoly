using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Only one citizen for Craft Structures (for simplicity) !
[RequireComponent(typeof(Target), typeof(Storage))]
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
	[SerializeField] private ParticleSystem smokeParticles;
	[SerializeField] private ParticleSystem fireParticles;

	[Header("Runtime")]
	public float fuel;
	public float progress;
	private bool resourcesUsed;
	public ItemType currentItemType;
	public Item craftedItem;
	public List<CraftOrder> orders;

	private bool isFueled;

	protected override void Awake()
	{
		base.Awake();
		list.Add(this);
		isFueled = fuelMax > 0;
		if (isFueled)
			storage.onItemsUpdate += Refuel;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		list.Remove(this);
		if (isFueled)
			storage.onItemsUpdate -= Refuel;
	}

	private void Start()
	{
		for (int i = 0; i < itemTypes.Count; i++)
			orders.Add(new CraftOrder(itemTypes[i], 0, true));
	}

	private void Update()
    {
		if (isFueled)
			Burn();

		if (target.ReservedBy != Player.instance && worker && worker.WorkTime() && worker.fsm.ActiveStateName == "Idle")
		{
			//Find first uncomplete order
			CraftOrder order = orders.Find( o => (!o.maintainAmount && o.count > 0) || (o.maintainAmount && o.count > storage.Count(o.itemType)) );
			if (order != null)
			{
				SetCurrentItemBlueprint(order.itemType);

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
						Debug.Log(missingFuel);
						if (missingFuel > 0)
						{
							Item fuelItem = null;
							Storage sourceStorage = null;

							if (!fuelItem)
								SearchFor.FuelInGatherStructures(transform.position, out fuelItem, out sourceStorage);
							if (!fuelItem)
								SearchFor.FuelInCraftStructures(transform.position, out fuelItem);
							if (!fuelItem)
								SearchFor.FuelInShopStructures(transform.position, out fuelItem, out sourceStorage);

							if (fuelItem)
								worker.fsm.Store(fuelItem, sourceStorage, storage, Mathf.FloorToInt(missingFuel / fuelItem.type.fuelValue) + 1);
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
							storage.DestroyItem(currentItemType.blueprint.requiredItems[i].type, currentItemType.blueprint.requiredItems[i].count);
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
		List<Item> fuelItems = storage.items.FindAll(i => !i.target.ReservedBy && i.type.fuelValue > 0).ToList();
		for(int i = fuelItems.Count - 1; i >= 0; i --)
		{
			fuel = Mathf.Min(fuelMax, fuel + fuelItems[i].type.fuelValue);
			storage.DestroyItem(fuelItems[i].type, 1);
		}
	}
	
	public void SetCurrentItemBlueprint(ItemType itemType)
	{
		currentItemType = itemType;
	}

}
