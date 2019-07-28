using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InputHints : MonoBehaviour
{
	[System.Serializable]
	private class Hint
	{
		public string axis;
		public string text;
		public Sprite icon;
		public Transform transformComponent;
		public Image imageComponent;
		public TextMeshProUGUI textComponent;
		public bool dontDestroy;
	}

	[System.Serializable]
	private class AxisIcons
	{
		public string axis;
		public Sprite keyboard;
		public Sprite controller;
	}


	public static InputHints instance;

	[SerializeField] private Transform listParent;
	[SerializeField] private Transform prefab;
	[SerializeField] private List<AxisIcons> axisIcons = new List<AxisIcons>();
	[SerializeField] private List<Hint> _hints = new List<Hint>();
	private static List<Hint> hints = new List<Hint>();


	private void Awake()
	{
		instance = this;
		_hints = hints;
	}

	private void LateUpdate()
	{
		for (int i = hints.Count - 1; i >= 0; i--)
			if (!hints[i].dontDestroy)
			{
				Destroy(hints[i].transformComponent.gameObject);
				hints.RemoveAt(i);
			}
			else
			{
				hints[i].dontDestroy = false;
			}
	}


	private static void SetHint(string axis, string text = null)
	{
		Hint hint = hints.Find(h => h.axis == axis);

		if (hint == null)
		{
			hint = new Hint();
			hint.axis = axis;
			hint.text = text == null ? axis : text;
			hint.transformComponent = Instantiate(instance.prefab, instance.listParent);
			hint.transformComponent.gameObject.SetActive(true);
			hint.imageComponent = hint.transformComponent.GetChild(0).GetComponent<Image>();
			hint.textComponent = hint.transformComponent.GetChild(1).GetComponent<TextMeshProUGUI>();
			hints.Add(hint);
		}
		else
		{
			hints.Remove(hint);
			hints.Add(hint);
			hint.transformComponent.SetAsLastSibling();
		}
		AxisIcons ai = instance.axisIcons.Find(a => a.axis == hint.axis);
		hint.icon = ai == null? null : (Game.controller? ai.controller : ai.keyboard);
		hint.imageComponent.sprite = hint.icon;
		hint.textComponent.text = text == null ? axis : text;
		hint.dontDestroy = true;
	}

	public static bool GetButtonDown(string axis, string text = null)
	{
		SetHint(axis, text);
		return Input.GetButtonDown(axis);
	}
}
