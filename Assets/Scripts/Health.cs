using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	[SerializeField][ProgressBar(0, "hpMax")]
	private float hp = 100;
	[SerializeField][MinValue(0)]
	private float hpMax = 100;
	public float HP
	{
		get => hp;
		private set
		{
			if (hp != value)
			{
				onHPChange?.Invoke(value);
				hp = Mathf.Max(value, 0);
			}
		}
	}
	public float HPMax
	{
		get => hpMax;
		private set => hpMax = Mathf.Max(value, 0);
	}

	public delegate void OnHPChange(float newHP);
	public event OnHPChange onHPChange;

	public bool Damage(float value)
	{
		HP -= value;
		if (HP == 0)
			Game.Depawn(gameObject);			
		return HP == 0;
	}

	public bool Repair(float value)
	{
		HP += value;
		return HP == HPMax;
	}
}
