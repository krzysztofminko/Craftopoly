using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Health))]
public class Citizen : MonoBehaviour, IMoney, IReserve
{
	public static List<Citizen> list = new List<Citizen>();

	[Header("Settings")]
	public bool DEBUG;

	public float walkSpeed = 1.0f;
	public float runSpeed = 2.0f;
	public float Money { get; set; } = 100;
	public TimeRange workTime;

	[Header("References")]
	public Animator animator;
	public Transform frontParent;
	public Transform handRParent;
	public Transform handLParent;
	public Transform beltLeftParent;

	[Header("Runtime")]
	public Workplace workplace;
	public House house;
	public Plot workPlot;
	[SerializeField] private Item _pickedItem;
	[SerializeField] private Item _attachedTool;
	public Item pickedItem
	{
		get => _pickedItem;
		set
		{
			_pickedItem = value;
			if (_pickedItem)
			{
				switch (_pickedItem.type.pickParent)
				{
					case ItemType.PickParent.Front: _pickedItem.SetParent(frontParent); break;
					case ItemType.PickParent.HandR: _pickedItem.SetParent(handRParent); break;
					case ItemType.PickParent.HandL: _pickedItem.SetParent(handLParent); break;
					default: _pickedItem.SetParent(transform); break;
				}
			}
		}
	}
	public Item attachedTool
	{
		get => _attachedTool;
		set
		{
			_attachedTool = value;
			if (_attachedTool)
			{
				switch (_attachedTool.type.attachParent)
				{
					case ItemType.AttachParent.None: Debug.LogError("AttachParent is None", this); _attachedTool.SetParent(transform); break;
					case ItemType.AttachParent.BeltLeft: _attachedTool.SetParent(beltLeftParent); break;
					default: _attachedTool.SetParent(transform); break;
				}
			}
		}
	}

	[ShowInInspector]
	public Citizen ReservedBy { get; set; }

	[HideInInspector]
	public FSMBinding fsm;
	[HideInInspector]
	public CharacterController characterController;
	[HideInInspector]
	public Skills skills;

	public Health Health{get; private set;}
	
	const int GetItemCallsMax = 10;
	int GetItemCalls;


	public virtual void Awake ()
	{
		list.Add(this);
		fsm = GetComponent<FSMBinding>();
		characterController = GetComponent<CharacterController>();
		skills = GetComponent<Skills>();
		Health = GetComponent<Health>();
	}
	
	private void OnDestroy()
	{
		list.Remove(this);
	}


	public bool WorkTime()
	{
		if(workTime.start > workTime.end)
			return VirtualTime.hour >= workTime.start || VirtualTime.hour < workTime.end;
		else
			return VirtualTime.hour >= workTime.start && VirtualTime.hour < workTime.end;
	}

	public bool GoTo(Transform transform, float? proximity = null)
	{
		return GoTo(transform.position, proximity == null? 1 : proximity.Value);
	}

	public bool GoTo(Vector3 position, float proximity)
	{
		if (Distance.Manhattan2D(transform.position, position) > proximity)
		{
			transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(position - transform.position, Vector3.up), Vector3.up);
			transform.position = Vector3.MoveTowards(transform.position, position, walkSpeed * Time.deltaTime);
			animator.SetBool("Walk", true);
			return false;
		}
		else
		{
			animator.SetBool("Walk", false);
			return true;
		}
	}
	
	public void Pay(IMoney payTo, float money)
	{
		Money -= money;
		payTo.Money += money;
	}

	public void AssignWorkplace(Workplace workplace)
	{
		if (workplace)
		{   //Change assingment
			if (this.workplace != workplace)
			{
				if (this.workplace)
					this.workplace.worker = null;
				if (workplace.worker)
					workplace.worker.AssignWorkplace(null);
				workplace.worker = this;
				this.workplace = workplace;
			}
		}
		else if (this.workplace)
		{   //Clear assignment
			this.workplace.worker = null;
			this.workplace = null;
		}
	}

	public void AssignHouse(House house)
	{
		if (house)
		{   //Change assingment
			int freeSlot = house.citizens.FindIndex(c => c == null);

			if (this.house != house && freeSlot > -1)
			{
				if (this.house)
					this.house.citizens[this.house.citizens.FindIndex(c => c == this)] = null;
				this.house = house;
				this.house.citizens[freeSlot] = this;
			}
		}
		else if (this.house)
		{   //Clear assignment
			this.house.citizens[this.house.citizens.FindIndex(c => c == this)] = null;
			this.house = null;
		}
	}

	public bool GetItems(List<ItemCount> items, Storage inStorage)
	{
		GetItemCalls++;
		if (DEBUG) Debug.Log("GetItems " + GetItemCalls);
		if (GetItemCalls > GetItemCallsMax)
		{
			GetItemCalls--;
			if (DEBUG) Debug.LogWarning(("Maximum of GetItem calls reached ({0})", GetItemCalls));
			return false;
		}

		for (int i = 0; i < items.Count; i++)
		{
			if (DEBUG) Debug.Log(String.Format("{0}, need: {1}, has {2}", items[i].type.name, items[i].count, inStorage.Count(items[i].type)));
			if (inStorage.Count(items[i].type) < items[i].count)
			{
				Item item = null;
				Storage sourceStorage = null;

				//if (supplyStorage && SearchFor.ItemInStorage(items[i].type, supplyStorage, out item))
				//	sourceStorage = supplyStorage;
				if (!item)
				{
					if (SearchFor.ItemInCraftStructures(items[i].type, inStorage.transform.position, out item))
						if (DEBUG) Debug.Log(String.Format("{0} in CraftStructure", item));
				}
				if (!item)
				{
					if (SearchFor.ItemInStorageStructures(items[i].type, inStorage.transform.position, out item, out sourceStorage))
						if (DEBUG) Debug.Log(String.Format("{0} in GatherStructure", item));
				}
				if (!item)
				{
					//TODO: Limit to the plot
					item = Item.free.Find(it => it.type == items[i].type);
					if(item)
						if (DEBUG) Debug.Log(String.Format("{0} lying on the ground.", item));
				}
				if (!item)
				{
					//TODO: Crafting must be accessed by some method, not copy-paste code (from Craft.cs) like here (same for Gathering)
					if (SearchFor.CraftStructureWithItemType(items[i].type, inStorage.transform.position, out CraftStructure craftStructure))
					{
						if (DEBUG) Debug.Log(String.Format("{0} in CraftStructure blueprints", items[i].type));
						List<ItemCount> missing = items[i].type.blueprint.MissingResources(craftStructure.storage);
						if (missing.Count > 0)
						{
							GetItemCalls--;
							return GetItems(missing, craftStructure.storage);
						}
						else
						{
							if (DEBUG) Debug.Log("Craft");
							craftStructure.currentItemType = items[i].type;
							fsm.Craft(items[i].type, craftStructure);
							GetItemCalls--;
							return true;
						}
					}
				}
				//TODO: Gather
				/*
				if (!item)
				{
					if(SearchFor.GatherStructureWithItemType(items[i].type, inStorage.transform.position, out GatherStructure gatherStructure))
					{

					}
				}
				*/
				if (!item)
				{
					//TODO: SearchFor.ItemTypeInPlots else increase itemType request in shop
					if (SearchFor.ItemInShopStructures(items[i].type, inStorage.transform.position, out item, out sourceStorage))
						if (DEBUG) Debug.Log(String.Format("{0} in ShopStructure", item));
				}

				//Store Item
				if (item)
				{
					if (DEBUG) Debug.Log("Store");
					fsm.Store(item, sourceStorage, inStorage);
					GetItemCalls--;
					return true;
				}
			}
		}

		if (DEBUG) Debug.Log("Can't get");
		GetItemCalls--;
		return false;
	}

}
