/*using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stats : MonoBehaviour
{
	const float initialValue = 100;
	const float initialMax = 100;

	public enum Name { Food, Tools }

	[System.Serializable]
	public class StatValue
	{
#if UNITY_EDITOR
		[HideInInspector]
		public string name;
#endif
		[HideInInspector]
		public Name stat;
		public float value = initialValue;
		public float max = initialMax;
	}

	[SerializeField]
	private List<StatValue> list;
	

	public float Get(Name name)
	{
		return list.Find(i => i.stat == name).value;
	}

	public float GetMax(Name name)
	{
		return list.Find(i => i.stat == name).max;
	}

	public void Set(Name stat, float value, bool addValue = false)
	{
		if (addValue || (!addValue && value >= 0))
		{
			StatValue sv = list.Find(s => s.stat == stat);
			sv.value = Mathf.Clamp((addValue ? sv.value : 0) + value, 0, sv.max);
		}
	}

	public void SetMax(Name stat, float maxValue, bool addValue = false)
	{
		if (addValue || (!addValue && maxValue >= 0))
		{
			StatValue sv = list.Find(s => s.stat == stat);
			if (sv == null)
				list.Add(new StatValue { stat = stat, max = maxValue });
			else
				sv.max = Mathf.Max((addValue ? sv.max : 0) + maxValue, 0);
		}
	}

	public Dictionary<Name, float> GetAll()
	{
		return list.ToDictionary(s => s.stat, s => s.value);
	}


#if UNITY_EDITOR	
	private StatValue[] backup;

	private void OnValidate()
	{
		if (backup != null && list.Count != backup.Length)
			list = backup.ToList();
		List<StatValue> newList = new List<StatValue>();
		foreach (Name e in System.Enum.GetValues(typeof(Name)))
		{
			StatValue nsv = new StatValue { name = e.ToString(), stat = e };
			StatValue sv = list.Find(i => i.name == e.ToString());
			nsv.value = sv == null? initialValue : sv.value;
			nsv.max = sv == null? initialMax : sv.max;
			newList.Add(nsv);			
		}

		list = newList;
		backup = new StatValue[list.Count];
		list.CopyTo(backup);
	}

	private void Reset()
	{
		List<StatValue> newList = new List<StatValue>();
		foreach (Name e in System.Enum.GetValues(typeof(Name)))
			newList.Add(new StatValue { name = e.ToString(), stat = e, value = initialValue , max = initialMax});
		list = newList;
		backup = new StatValue[list.Count];
		list.CopyTo(backup);
	}
#endif
}
*/