using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skills : MonoBehaviour
{
	public enum Name { Building, GatherStone, GatherWood, Fight, CraftTools }  //...
	public const int maxSkillValue = 5;

	[System.Serializable]
	public class SkillValue
	{
		public Name name;
		[Range(0, maxSkillValue)]
		public int value = 0;
	}

	[SerializeField]
	private List<SkillValue> list;

	public void Set(Name skill, int value, bool addValue = false)
	{
		if (addValue || (!addValue && value >= 0))
		{
			SkillValue sv = list.Find(s => s.name == skill);
			if (sv == null)
				list.Add(new SkillValue { name = skill, value = value });
			else
				sv.value = Mathf.Clamp((addValue ? sv.value : 0) + value, 0, maxSkillValue);
			if (sv.value < 1)
				list.Remove(sv);
		}
	}

	public int Get(Name skill)
	{
		SkillValue sv = list.Find(s => s.name == skill);
		return sv != null ? sv.value : 0;
	}

	public List<SkillValue> GetAll()
	{
		return list;
	}

}
