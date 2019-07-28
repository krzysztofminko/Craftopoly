using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utilities.UI
{
	//TODO: Multipanel Focus
	public class MessageForm : MonoBehaviour
	{
		public static MessageForm instance;

		[Header("References")]
		public TextMeshProUGUI messageText;
		public RectTransform buttonsList;
		public Button buttonPrefab;

		private RectTransform panel;
		private RectTransform linkedTo;

		private void Awake()
		{
			instance = this;
			panel = transform.GetChild(0).GetComponent<RectTransform>();
			Hide();
		}

		private void Update()
		{
			if (linkedTo && !linkedTo.gameObject.activeSelf)
				Hide();
			if (Input.GetButtonDown("Cancel"))
				Hide();
		}

		public void Show(string message, Dictionary<string, UnityAction> buttons, RectTransform linkedTo = null, int defaultButtonId = 0)
		{
			messageText.text = message;
			this.linkedTo = linkedTo;

			int i;
			for (i = 0; i < buttonsList.childCount; i++)
				buttonsList.GetChild(i).gameObject.SetActive(false);

			i = 0;
			Button button;
			foreach (var b in buttons)
			{
				if (i >= buttonsList.childCount)
					button = Instantiate(buttonPrefab, buttonsList);
				else
					button = buttonsList.GetChild(i).GetComponent<Button>();

				button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = b.Key;
				button.gameObject.SetActive(true);

				int id = i;
				Button buttonLocal = button;
				button.onClick.RemoveAllListeners();
				if(b.Value != null)
					button.onClick.AddListener(b.Value);
				button.onClick.AddListener(delegate { Hide(); });

				i++;
			}

			panel.gameObject.SetActive(true);

			buttonsList.GetChild(defaultButtonId).GetComponent<Button>().Select();
		}

		private void Hide()
		{
			panel.gameObject.SetActive(false);
		}
	}
}