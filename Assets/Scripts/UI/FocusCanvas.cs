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

		public Target target;

		[Header("References")]
		public TextMeshProUGUI nameText;
		public Slider hpBar;
		public TextMeshProUGUI hpText;
		public TextMeshProUGUI moneyText;

		Canvas canvas;

		private void Awake()
		{
			instance = this;
			canvas = GetComponent<Canvas>();
		}

		private void Update()
		{
			if (target)
			{
				hpBar.maxValue = target.hpMax;
				hpBar.value = target.hp;
				hpText.text = target.hp + "/" + target.hpMax;

				moneyText.gameObject.SetActive(target.structure && target.structure.plot);
				if (target.structure)
				{
					if (target.structure.plot)
					{
						moneyText.text = "Plot money: " + target.structure.plot.Money.ToString("0.00");
					}
				}
			}
		}

		public void Show(Target target)
		{
			this.target = target;
			nameText.text = target.name + (target.item? " x" + target.item.count : "");

			transform.GetChild(0).gameObject.SetActive(true);
		}

		public void Hide()
		{
			transform.GetChild(0).gameObject.SetActive(false);
			target = null;
		}
	}
}