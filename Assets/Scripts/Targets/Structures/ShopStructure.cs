using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Target), typeof(Storage))]
public class ShopStructure : Workplace
{
	public static new List<ShopStructure> list = new List<ShopStructure>();

	[Header("Settings")]
	[Header("ShopStructure")]
	public float resupplyDelay = 1;
	public Storage supplyStorage;
	public List<SupplyOrder> orders;

	[Header("Runtime")]
	public bool shopkeeperAvailable;

	float timeFromLastTransaction;
	bool supply;
	SupplyOrder supplyOrder;


	protected override void Awake()
	{
		base.Awake();
		list.Add(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		list.Remove(this);
	}

	private void Update()
	{
		if (!worker)
		{
			shopkeeperAvailable = false;
		}
		else
		{
			shopkeeperAvailable = Distance.Manhattan2D(worker.transform.position, transform.position) < 2;

			//Is during transaction?
			if (target.ReservedBy)
			{
				if (worker.fsm.ActiveStateName == "Idle")
					worker.fsm.GoTo(transform.position);
			}
			else
			{
				if (timeFromLastTransaction < resupplyDelay)
				{
					//Count time from last transaction
					timeFromLastTransaction += Time.deltaTime;
				}
				else if (worker.WorkTime() && worker.fsm.ActiveStateName == "Idle")
				{//Resupply Items for sale

					if (!supply)
					{
						supplyOrder = orders.Find(o => storage.Count(o.type) < o.min);
						if (supplyOrder != null)
							supply = true;
					}
					else
					{
						if (storage.Count(supplyOrder.type) >= supplyOrder.max)
							supply = false;
					}

					if (supply)
					{
						int missingCount = supplyOrder.max - storage.Count(supplyOrder.type);

						Item item = null;
						Storage sourceStorage = null;

						if (supplyStorage && SearchFor.ItemInStorage(supplyOrder.type, supplyStorage, out item))
							sourceStorage = supplyStorage;
						if (!item)
							SearchFor.ItemInStorageStructures(supplyOrder.type, transform.position, out item, out sourceStorage);
						if (!item)
							SearchFor.ItemInCraftStructures(supplyOrder.type, transform.position, out item);
						//if (!item)
							//SearchFor.ItemInShopStructures(supplyOrder.type, transform.position, out item, out sourceStorage);
							
						//Store Item
						if (item)
							worker.fsm.Store(item, sourceStorage, storage);
					}
				}
			}
		}
	}


	public void StartTransaction(Citizen client)
	{
		target.ReservedBy = client;
	}

	public void FinishTransaction()
	{
		target.ReservedBy = null;
		timeFromLastTransaction = 0;
	}

}

