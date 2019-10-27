using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization : MonoBehaviour
{
	static Localization instance;

	public delegate void OnSetLanguage();
	public static event OnSetLanguage onSetLanguage;

	[Header("Settings")]
	public TextAsset languageFile;
	public bool debugLog;
	
	private static Dictionary<string, string>  dictionary = new Dictionary<string, string>();

    void Awake()
    {
		if (instance)
			Debug.LogError("There can be only one instance of Localization class.", instance);
		else
		{
			instance = this;
			SetLanguage(languageFile);
		}
	}

	public static void SetLanguage(TextAsset languageFile)
	{
		instance.languageFile = languageFile;
		int l = 0;
		string[] split = languageFile.text.Split(";"[0], "\n"[0]);
		for (var i = 0; i < split.Length / 2; i++)
		{
			if (!split[i * 2].StartsWith("#"))
			{
				dictionary.Add(split[i * 2].ToUpper(), split[i * 2 + 1].Replace("\n", "").Replace("\r", ""));
				l++;
			}
		}
		onSetLanguage?.Invoke();
	}

	public static string Translate(string text)
	{
		text = text.ToUpper();
		if (instance.languageFile && dictionary.ContainsKey(text.ToUpper()))
		{
			if (instance.debugLog)
				Debug.Log(text + ":" + dictionary[text]);
			return (dictionary[text]);
		}
		else
		{
			if (instance.debugLog)
				Debug.Log(text + ":<no translation>");
			return (text);
		}
	}
}
