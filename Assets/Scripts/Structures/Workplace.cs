using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Workplace : Structure
{
	public static new List<Workplace> list = new List<Workplace>();
	
	[Header("Runtime")]
	[Header("Workplace")]
	public Citizen worker;

	protected override void Awake()
	{
		base.Awake();
		list.Add(this);
	}

	protected override void OnDestroy()
	{
		base.Awake();
		list.Remove(this);
	}

}
