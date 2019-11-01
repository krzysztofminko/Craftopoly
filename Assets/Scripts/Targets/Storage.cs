using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{	
	public static List<Storage> list = new List<Storage>();
	
	public delegate void OnItemsUpdate();
	public event OnItemsUpdate onItemsUpdate;

	[Header("References")]
	[SerializeField] private Transform inputTransform;
	[SerializeField] private Transform releaseTransform;

	[Header("Runtime")]
	public List<Item> items;

	[HideInInspector]
	public Target target;

	private void Awake()
	{
		list.Add(this);
		target = GetComponent<Target>();
	}

	private void OnDestroy()
	{
		list.Remove(this);

		//Free items
		for (int i = 0; i < items.Count; i++)
			items[i].SetParent(null);
	}
	

	public Item AddItem(Item item)
	{
		Item itemInside = items.Find(i => i.type == item.type);
		if (itemInside)
		{
			itemInside.target.ReservedBy = null;	//TODO: Why?
			itemInside.count += item.count;
			Destroy(item.gameObject);
			onItemsUpdate?.Invoke();
			return itemInside;
		}
		else
		{
			items.Add(item);
			item.SetParent(inputTransform? inputTransform : transform);
			onItemsUpdate?.Invoke();
			return item;
		}
	}

	public Item RemoveItem(Item item, int count = 0)
	{
		if (count == 0)
			count = item.count;

		count = Mathf.Max(count, 1);

		if (item.count - count > 0)
		{
			item.count -= count;
			Item i = item.type.Spawn(count, transform.position, transform.rotation);
			if (releaseTransform)
				i.transform.position = releaseTransform.position;
			i.SetParent(null);
			onItemsUpdate?.Invoke();
			return i;
		}
		else
		{
			if (releaseTransform)
				item.transform.position = releaseTransform.position;
			item.SetParent(null);
			items.Remove(item);
			onItemsUpdate?.Invoke();
			return item;
		}
	}

	public void DestroyItem(ItemType itemType, int count = 0)
	{
		Item item = items.Find(i => i.type == itemType);
		if (item)
		{
			if (count == 0)
				count = item.count;

			item.count -= count;
			if (item.count == 0)
			{
				items.Remove(item);
				Destroy(item.gameObject);
			}
			else if(item.count < 0)
			{
				Debug.LogError("Count parameter is to big than actual count of item in this storage.", this);
			}
			onItemsUpdate?.Invoke();
		}
		else
		{
			Debug.LogError($"{itemType} does not exist in this storage.", this);
		}
	}

	public int Count(ItemType itemType)
	{
		for (int i = 0; i < items.Count; i++)
			if (!items[i])
				Debug.Log("!");
			else if (items[i].type == itemType)
				return items[i].count;
		return 0;
	}

}
