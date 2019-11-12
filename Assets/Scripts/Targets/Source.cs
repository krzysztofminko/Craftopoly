using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Source : MonoBehaviour, IReserve {

	public static List<Source> list = new List<Source>();

	[Header("Settings")]
	public ItemType itemType;
	public int count;

	[HideInInspector]
	public Target target;

	public Health Health { get; private set; }
	
	[ShowInInspector]
	public Citizen ReservedBy { get; set; }

	private void Awake()
	{
		target = GetComponent<Target>();
		Health = GetComponent<Health>();
		list.Add(this);
	}

	public Item Gather(float value)
	{
		return Health.Damage(value)? itemType.Spawn(count, transform.position, transform.rotation) : null;
	}

	private void OnDestroy()
	{
		list.Remove(this);
	}
}
