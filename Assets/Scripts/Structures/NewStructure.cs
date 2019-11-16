using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Storage))]
public class NewStructure : Workplace
{
	[Header("Settings")]
	public float range = 100;

	[Header("References")]
	public Transform[] corners;

	[Header("Runtime")]
	public float progress;

	[Header("Private")]
	[SerializeField]
	private List<ItemCount> missing = new List<ItemCount>();
	[SerializeField]
	private List<Storage> storages = new List<Storage>();
	[SerializeField]
	private List<Item> items = new List<Item>();
	[SerializeField]
	private List<Source> sources = new List<Source>();
	
	bool created;


	protected override void Awake()
	{
		base.Awake();
		list.Remove(this);
		GetComponent<Collider>().enabled = false;
	}

	private void _Update()
	{
		//TODO: Rewrite NewStructure with new rules
		/*
		if (created && next)
		{
			//Manage
			if (worker && worker.fsm.ActiveStateName == "Idle")
			{
				missing = next.GetComponent<Blueprint>().MissingResources(storage);

				int searchFor = -1;
				if (missing.Count > 0)
				{
					//Search storages
					if (missing[0].type.value * missing[0].count <= worker.money)
					{
						storages = Storage.list.FindAll(s => s != this && s.target.hp > 0 && Distance.Manhattan2D(transform.position, s.transform.position) < range && s.Count(missing[0].type) > 0).OrderBy(s => Distance.Manhattan2D(transform.position, s.transform.position)).ToList(); ;
						if (storages.Count > 0)
						{
							searchFor = 0;
							break;
						}
					}
					else
					{
						items = Item.free.FindAll(r => r.type == missing[0].type && !r.target.ReservedBy && Distance.Manhattan2D(transform.position, r.transform.position) < range).OrderBy(r => Distance.Manhattan2D(transform.position, r.transform.position)).ToList();
						if (items.Count > 0)
						{
							searchFor = 0;
							break;
						}
						else if (!missing[0].type.GetComponent<Blueprint>())
						{
							sources = Source.list.FindAll(s => s.item == missing[0].type && !s.target.ReservedBy && s.target.hp > 0 && Distance.Manhattan2D(transform.position, s.transform.position) < range).OrderBy(s => Distance.Manhattan2D(transform.position, s.transform.position)).ToList();
							if (sources.Count > 0)
							{
								searchFor = 0;
								break;
							}
						}
					}
				}

				if (missing.Count > 0)
				{
					if (searchFor > -1)
					{
						//Get missing
						if (storages.Count > 0)
							worker.Store(storages[0].items.Find(i => i.type == missing[searchFor].type), storage, missing[searchFor].count);
						else if (items.Count > 0)
							worker.Store(items[0], storage, missing[searchFor].count);
						else if (sources.Count > 0)
							worker.Gather(sources[0], storage, missing[searchFor].count);
					}
				}
				else
				{
					//Craft
					worker.Build(this);
				}
			}
		}*/
	}



	public void Create()
	{
		list.Add(this);
		created = true;
		GetComponent<Collider>().enabled = true;
	}
}
