using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupplyOrder
{
	[Tooltip("Supplied type of item")]
	public ItemType type;
	[Tooltip("When item.count < min then start supply")]
	public int min;
	[Tooltip("When item.count >= max then end supply")]
	public int max;
}
