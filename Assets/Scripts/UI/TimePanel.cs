using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Localization;

namespace UI
{
	public class TimePanel : MonoBehaviour
	{
		public TextMeshProUGUI dayText;
		public TextMeshProUGUI hourText;

		void Update()
		{
			dayText.text = Translate("DAY_LABEL") + " " + VirtualTime.day;
			hourText.text = Translate("HOUR_LABEL") + " " + (int)VirtualTime.hour;
		}
	}
}