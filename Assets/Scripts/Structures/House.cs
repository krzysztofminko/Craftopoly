using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Storage))]
public class House : Structure
{
	public float value;

	public List<Citizen> citizens;

	private void Update()
	{		
		for (int i = 0; i < citizens.Count; i++)
			if (citizens[i] && citizens[i].fsm.ActiveStateName == "Idle")
			{
				if(!citizens[i].WorkTime())
				{
					
				}
				else
				{

				}
			}
	}
}
