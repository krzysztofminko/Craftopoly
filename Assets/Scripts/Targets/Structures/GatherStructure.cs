using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Target))]
public class GatherStructure : Workplace
{
	public static new List<GatherStructure> list = new List<GatherStructure>();

	[Header("Settings")]
	[Header("GatherStructure")]
	public float rangeOfSearch = 100;
	public ItemType itemType;

	[Header("Private")]
	[SerializeField]
	private List<Item> items = new List<Item>();
	[SerializeField]
	private List<Source> sources = new List<Source>();

	private float delayNextSearch;


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
		if (delayNextSearch > 0)
		{
			delayNextSearch -= Time.deltaTime;
		}
		else if (worker && worker.WorkTime() && worker.fsm.ActiveStateName == "Idle")
		{
			//Find items on the ground
			items = Item.free.FindAll(r => r.type == itemType && !r.ReservedBy && Distance.Manhattan2D(transform.position, r.transform.position) < rangeOfSearch).OrderBy(r => Distance.Manhattan2D(transform.position, r.transform.position)).ToList();
			if (items.Count > 0)
			{
				if (SearchFor.NearestStorageStructure(plot, transform.position, out StorageStructure targetStorage))
					worker.fsm.Store(items[0], null, targetStorage.storage);
				else
					Debug.LogError("No StorageStructure on plot");
			}
			else
			{
				//Find sources
				sources = Source.list.FindAll(s => s.itemType == itemType && !s.ReservedBy && s.Health.HP > 0 && Distance.Manhattan2D(transform.position, s.transform.position) < rangeOfSearch).OrderBy(s => Distance.Manhattan2D(transform.position, s.transform.position)).ToList();
				if (sources.Count > 0)
				{
					worker.fsm.Gather(sources[0], storage);
				}
				else
				{
					delayNextSearch = 10;
				}
			}
		}
	}
}
