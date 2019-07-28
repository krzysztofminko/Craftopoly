using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
	private static List<Item> list = new List<Item>();
	public static List<Item> free = new List<Item>();
	private static bool prefabThumbnailsGenerated;

	[Header("Runtime")]
	public ItemType type;
	public int count = 1;
	public float durability;

	//[HideInInspector]
	private Texture2D thumbnail;
	//[HideInInspector]
	private Sprite thumbnailSprite;

	[HideInInspector]
	public Target target;
	
	private void Awake()
	{		
		target = GetComponent<Target>();
		list.Add(this);
		SetParent(null);
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
