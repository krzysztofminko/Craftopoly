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
	

	public void AddItem(Item item)
	{
		items.Add(item);
		item.SetParent(inputTransform ? inputTransform : transform);
		onItemsUpdate?.Invoke();
	}

	public Item RemoveItem(Item item)
	{
		if (releaseTransform)
			item.transform.position = releaseTransform.position;
		item.SetParent(null);
		items.Remove(item);
		onItemsUpdate?.Invoke();
		return item;
	}

	public void DestroyItemType(ItemType itemType, int count = 0)
	{
		int inStorage = Count(itemType);
		if(inStorage == 0)
			Debug.LogError($"{itemType} does not exist in this storage.", this);

		if (count > inStorage)
			Debug.LogError("Count parameter is to big than actual count of item in this storage.", this);

		if (count < 0)
			Debug.LogError($"Count parameter can't be smaller than zero {count}.", this);

		if (count == 0)
			count = inStorage;
		
		for(int i = count - 1; i >= 0; i--)
		{
			if(items[i].type == itemType)
			{
				Destroy(items[i].gameObject);
				items.RemoveAt(i);
			}
		}

		onItemsUpdate?.Invoke();		
	}
	
	public int Count(ItemType itemType)
	{
		int count = 0;
		for (int i = 0; i < items.Count; i++)
			if (!items[i])
				Debug.LogError("!");
			else if (items[i].type == itemType)
				count++;
		return count;
	}

}
