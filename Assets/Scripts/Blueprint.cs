using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Blueprint
{
	public float duration = 1;
	public List<ItemType> requiredTools;
	public List<ItemCount> requiredItems;

	public List<ItemCount> MissingResources(Storage storage)
	{
		List<ItemCount> list = new List<ItemCount>();
		for(int i = 0; i < requiredItems.Count; i++)
		{
			int count = requiredItems[i].count - storage.Count(requiredItems[i].type);
			if (count > 0)
				list.Add(new ItemCount(requiredItems[i].type, count));
		}

		return list;
	}
}
