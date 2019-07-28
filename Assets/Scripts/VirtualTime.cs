using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualTime : MonoBehaviour
{

	[SerializeField]
	private int _startDay;
	[SerializeField]
	private float _startHour;
	[SerializeField]
	[Tooltip("In seconds")]
	private float _hourDuration = 1;
	[SerializeField]
	[Tooltip("In game hours")]
	private float _nightDuration = 8;

	public static int day;
	public static float hour;
	public static float hourDuration;
	public static float nightDuration;

	public bool Daytime
	{
		get { return (hour > 0 && hour < nightDuration * 0.5f) || (hour > 24 - nightDuration * 0.5f && hour < 24); }
	}

	void Awake()
	{
		day = _startDay;
		hour = _startHour;
		hourDuration = _hourDuration;
		nightDuration = _nightDuration;
	}

	void Update()
    {
		hour += Time.deltaTime / hourDuration;
		if (hour >= 24)
		{
			hour = 0;
			day++;
		}
	}
}
