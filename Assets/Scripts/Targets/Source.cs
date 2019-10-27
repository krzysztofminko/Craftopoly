using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : MonoBehaviour {

	public static List<Source> list = new List<Source>();

	[Header("Settings")]
	public ItemType itemType;
	public int count;

	[HideInInspector]
	public Target target;

	private void Awake()
	{
		target = GetComponent<Target>();
		list.Add(this);
	}

	public Item Gather(float value)
	{
		if (target.Damage(value))
			return itemType.Spawn(count, transform.position, transform.rotation);
		return null;
	}

	private void OnDestroy()
	{
		list.Remove(this);
	}
}
