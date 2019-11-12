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
	
	[Header("Runtime")]
	public ItemType type;
	public float durability;
	
	private Texture2D thumbnail;
	private Sprite thumbnailSprite;

	[ShowInInspector]
	public Citizen ReservedBy { get; set; }

	private void Awake()
	{		
		list.Add(this);
	}
	
	private void OnDestroy()
	{
		list.Remove(this);
		free.Remove(this);
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
