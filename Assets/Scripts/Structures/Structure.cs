﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health), typeof(FocusTarget))]
public abstract class Structure : MonoBehaviour, IReserve
{	
	public static List<Structure> list = new List<Structure>();

	public delegate void OnStructureAwake(Structure structure);
	public static event OnStructureAwake onStructureAwake;
	public delegate void OnStructureDestroy(Structure structure);
	public static event OnStructureDestroy onStructureDestroy;

	[Header("Settings")]
	public Blueprint blueprint;
	public Structure next;

	[Header("Runtime")]
	public Plot plot;
	
	[HideInInspector]
	public Storage storage;
	public Health Health { get; private set; }
	public FocusTarget FocusTarget { get; private set; }
	
	[ShowInInspector]
	public Citizen ReservedBy { get; set; }

	protected virtual void Awake()
	{
		storage = GetComponent<Storage>();
		Health = GetComponent<Health>();
		FocusTarget = GetComponent<FocusTarget>();
		list.Add(this);
		onStructureAwake?.Invoke(this);
	}

	protected virtual void OnDestroy()
	{
		list.Remove(this);
		onStructureDestroy?.Invoke(this);
	}
}
