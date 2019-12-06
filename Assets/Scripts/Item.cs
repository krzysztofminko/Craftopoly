using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour, IReserve
{
	private static List<Item> list = new List<Item>();
	[ShowInInspector]
	public static List<Item> free = new List<Item>();
	private static bool prefabThumbnailsGenerated;

	public delegate void OnItemSpawn(Item item);
	public static event OnItemSpawn onItemSpawn;
	public delegate void OnItemDespawn(Item item);
	public static event OnItemDespawn onItemDespawn;

	[Header("Runtime")]
	public ItemType type;
	public float durability;
	
	private Texture2D thumbnail;
	private Sprite thumbnailSprite;

	[ShowInInspector]
	public Citizen ReservedBy { get; set; }

	private bool spawned;

	private void Awake()
	{
		if (!spawned)
		{
			OnSpawn();
			spawned = true;
		}
	}

	public void OnSpawn()
	{
		list.Add(this);
		onItemSpawn?.Invoke(this);
	}
	
	public void OnDespawn()
	{
		list.Remove(this);
		free.Remove(this);
		onItemDespawn?.Invoke(this);
	}

	public void SetParent(Transform parent, Vector3? localPosition = null, Quaternion? localRotation = null)
	{
		transform.parent = parent;
		if (parent)
		{
			transform.GetComponent<Rigidbody>().isKinematic = true;
			transform.GetComponent<Collider>().enabled = false;
			transform.localPosition = localPosition == null? Vector3.zero : localPosition.Value;
			transform.localRotation = localRotation == null? Quaternion.identity : localRotation.Value;
			free.Remove(this);
		}
		else
		{
			transform.GetComponent<Rigidbody>().isKinematic = false;
			transform.GetComponent<Collider>().enabled = true;
			if (!free.Contains(this))
				free.Add(this);
		}
	}
}
