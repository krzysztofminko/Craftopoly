using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Source : MonoBehaviour, IReserve, ISpawnable
{
	public static List<Source> list = new List<Source>();

	public delegate void OnSourceSpawn(Source source);
	public static event OnSourceSpawn onSourceSpawn;
	public delegate void OnSourceDespawn(Source source);
	public static event OnSourceDespawn onSourceDespawn;
	

	[Header("Settings")]
	public ItemType itemType;
	public int count;
	
	public Health Health { get; private set; }
	
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
		Health = GetComponent<Health>();
		list.Add(this);
		onSourceSpawn?.Invoke(this);
	}

	public void OnDespawn()
	{
		list.Remove(this);
		onSourceDespawn?.Invoke(this);
	}

	public Item Gather(float value)
	{
		return Health.Damage(value)? itemType.Spawn(count, transform.position, transform.rotation) : null;
	}
}
