using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Target), typeof(Health), typeof(FocusTarget))]
public abstract class Structure : MonoBehaviour, IReserve
{	
	public static List<Structure> list = new List<Structure>();

	[Header("Settings")]
	public Blueprint blueprint;
	public Structure next;

	[Header("Runtime")]
	public Plot plot;
	
	[HideInInspector]
	public Storage storage;	
	[HideInInspector]
	public Target target;
	public Health Health { get; private set; }
	public FocusTarget FocusTarget { get; private set; }
	
	[ShowInInspector]
	public Citizen ReservedBy { get; set; }

	protected virtual void Awake()
	{
		target = GetComponent<Target>();
		storage = GetComponent<Storage>();
		Health = GetComponent<Health>();
		FocusTarget = GetComponent<FocusTarget>();
		list.Add(this);
	}

	protected virtual void OnDestroy()
	{
		list.Remove(this);
	}
}
