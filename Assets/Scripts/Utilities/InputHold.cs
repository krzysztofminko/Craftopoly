using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputHold : MonoBehaviour
{
	private static InputHold instance;

	[System.Serializable]
	public class AxisTimer
	{
		public string axis;
		public float timer;
		public bool reset;
	}
	public List<AxisTimer> axesTimers = new List<AxisTimer>();

	void Awake()
	{
		instance = this;
		
		//Get Axes
		SerializedObject obj = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axisArray = obj.FindProperty("m_Axes");

		for (int i = 0; i < axisArray.arraySize; ++i)
		{
			string axis = axisArray.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue;
			if(axesTimers.Find(a => a.axis == axis) == null)
				axesTimers.Add(new AxisTimer { axis = axis });
		}
	}

	void LateUpdate()
    {
        for(int i = 0; i < axesTimers.Count; i++)
		{
			if (Input.GetButton(axesTimers[i].axis) && !axesTimers[i].reset)
			{
				axesTimers[i].timer += Time.deltaTime;
			}
			else
			{
				axesTimers[i].timer = 0;
			}
			if(Input.GetButtonUp(axesTimers[i].axis))
				axesTimers[i].reset = false;
		}
    }


	public static bool GetButtonHold(string axis, float duration = 1)
	{
		AxisTimer at = instance.axesTimers.Find(a => a.axis == axis);
		if (at == null)
			Debug.LogError("Axis '" + axis + "' doesn't exist.");
		if(at.timer >= duration)
		{
			at.reset = true;
			return true;
		}
		return false;
	}
}
