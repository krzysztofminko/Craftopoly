using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class FocusCanvas : MonoBehaviour
	{
		public static FocusCanvas instance;

		public FocusTarget focusTarget;

		[Header("References")]
		public TextMeshProUGUI nameText;
		public Slider hpBar;
		public TextMeshProUGUI hpText;
		public TextMeshProUGUI infoText;

		private Canvas canvas;
		private Health health;


		private void Awake()
		{
			instance = this;
			canvas = GetComponent<Canvas>();
			canvas.enabled = false;
		}

		private void Update()
		{
			if (focusTarget)
			{
				hpBar.gameObject.SetActive(health);
				hpText.gameObject.SetActive(health);
				if (health)
				{
					hpBar.maxValue = health.HPMax;
					hpBar.value = health.HP;
					hpText.text = health.HP + "/" + health.HPMax;
				}
				
				infoText.text = focusTarget.info;
			}
		}

		public void Show(FocusTarget focusTarget)
		{
			this.focusTarget = focusTarget;
			health = focusTarget.GetComponent<Health>();
			nameText.text = focusTarget.name;

			canvas.enabled = true;
		}

		public void Hide()
		{
			canvas.enabled = false;
			focusTarget = null;
		}
	}
}