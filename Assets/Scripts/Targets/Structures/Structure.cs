using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Target), typeof(Health))]
public abstract class Structure : MonoBehaviour
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

	protected virtual void Awake()
	{
		target = GetComponent<Target>();
		storage = GetComponent<Storage>();
		Health = GetComponent<Health>();
		list.Add(this);
	}

	protected virtual void OnDestroy()
	{
		list.Remove(this);
	}
}
