using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
	[Header("For static text only")]
	public string textId;

	private TextMeshProUGUI textComponent;

	private void Start()
	{
		textComponent = GetComponent<TextMeshProUGUI>();
		Translate();
		Localization.onSetLanguage += Translate;
	}

	private void OnDestroy()
	{
		Localization.onSetLanguage -= Translate;
	}

	private void Translate()
	{
		textComponent.text = Localization.Translate(textId);
	}
}
