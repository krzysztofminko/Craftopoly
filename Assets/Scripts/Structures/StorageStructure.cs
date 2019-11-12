using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageStructure : Structure
{
	public static new List<StorageStructure> list = new List<StorageStructure>();

	protected override void Awake()
	{
		base.Awake();
		list.Add(this);
		if (plot)
			plot.storageStructures.Add(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		list.Remove(this);
		if (plot)
			plot.storageStructures.Remove(this);
	}

}
