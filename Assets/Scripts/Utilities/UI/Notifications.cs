using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Utilities.UI
{
	public class Notifications : MonoBehaviour
	{
		public static Notifications instance;

		[Header("Settings")]
		[Tooltip("Ignore if equals 0")]
		public int limit;
		[Tooltip("Ignore if equals 0")]
		public float duration;

		[Header("References")]
		public RectTransform notificationsList;
		public TextMeshProUGUI notificationPrefab;

		private List<float> timers = new List<float> { 0};


		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
			for(int i = 1; i < notificationsList.childCount; i++)
			{
				timers[i] -= Time.deltaTime;
				Color color = notificationsList.GetChild(i).GetComponent<TextMeshProUGUI>().color;
				color = new Color(color.r, color.g, color.b, timers[i] > duration * 0.5f? 1.0f : timers[i] / (duration * 0.5f));
				notificationsList.GetChild(i).GetComponent<TextMeshProUGUI>().color = color;
				if (timers[i] < 0)
					Delete(i);
			}
		}


		public void Show(bool toggle)
		{
			transform.GetChild(0).gameObject.SetActive(toggle);
		}

		public void Add(string message)
		{
			TextMeshProUGUI n = Instantiate(notificationPrefab, notificationsList);
			n.text = message;
			n.gameObject.SetActive(true);
			timers.Add(duration);
		}

		private void Delete(int id)
		{
			timers.RemoveAt(id);
			DestroyImmediate(notificationsList.GetChild(id).gameObject);
		}
	}
}