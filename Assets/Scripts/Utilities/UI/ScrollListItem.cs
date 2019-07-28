
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities.UI
{
	public class ScrollListItem : Selectable
	{
		EventTrigger eventTrigger;
		[HideInInspector]
		public EventTrigger.Entry selectEntry;
		[HideInInspector]
		public EventTrigger.Entry pointerEntry;

		new void Awake()
		{
			AddTriggers();
		}

		new void Reset()
		{
			AddTriggers();
		}

		void AddTriggers()
		{
			eventTrigger = GetComponent<EventTrigger>();
			if (!eventTrigger)
				eventTrigger = gameObject.AddComponent<EventTrigger>();

			selectEntry = eventTrigger.triggers.Find(t => t.eventID == EventTriggerType.Select);
			if (selectEntry == null)
				eventTrigger.triggers.Add(selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select });

			pointerEntry = eventTrigger.triggers.Find(t => t.eventID == EventTriggerType.PointerClick);
			if (pointerEntry == null)
				eventTrigger.triggers.Add(pointerEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick });
		}

	}
}