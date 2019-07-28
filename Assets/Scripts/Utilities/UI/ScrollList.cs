using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities.UI
{
	//TODO: Categories and filters
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollList : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[Header("References")]
		[SerializeField]
		RectTransform listParent;

		public delegate void onListItemCreate(int id, ScrollListItem listItem);
		public event onListItemCreate OnListItemCreate;
		public delegate void onListItemSelect(int id, ScrollListItem listItem);
		public event onListItemSelect OnListItemSelect;

		ScrollRect scrollRect;
		ScrollListItem listItem;
		ScrollListItem listItemPrefab;
		int selectedId = -1;
		Transform updateScrollToTransform;
		string hAxis;
		string vAxis;
		bool pointerOver;


		private void Awake()
		{
			listItemPrefab = listParent.GetChild(0).GetComponent<ScrollListItem>();
			hAxis = EventSystem.current.GetComponent<StandaloneInputModule>().horizontalAxis;
			vAxis = EventSystem.current.GetComponent<StandaloneInputModule>().verticalAxis;
			scrollRect = GetComponent<ScrollRect>();
			scrollRect.verticalScrollbar.onValueChanged.AddListener(delegate { SelectSelected(); });
		}

		private void Update()
		{
			if(pointerOver)
				scrollRect.verticalNormalizedPosition += Input.mouseScrollDelta.y / listParent.rect.height * scrollRect.scrollSensitivity * 2;

			if (updateScrollToTransform)
			{
				if (updateScrollToTransform.localPosition.y + listParent.localPosition.y > -listItemPrefab.GetComponent<RectTransform>().sizeDelta.y)
					scrollRect.verticalNormalizedPosition += Time.deltaTime / listParent.childCount * 20;
				else if (updateScrollToTransform.localPosition.y + listParent.localPosition.y < -scrollRect.GetComponent<RectTransform>().sizeDelta.y + listItemPrefab.GetComponent<RectTransform>().sizeDelta.y)
					scrollRect.verticalNormalizedPosition -= Time.deltaTime / listParent.childCount * 20;
				else
					updateScrollToTransform = null;
				if (scrollRect.verticalNormalizedPosition < 0 || scrollRect.verticalNormalizedPosition > 1)
				{
					updateScrollToTransform = null;
					scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition, 0.0f, 1.0f);
				}
			}
		}


		public void Draw(int count, int? selectDefaultId = null)
		{
			for (int i = count; i < listParent.childCount; i++)
				listParent.GetChild(i).gameObject.SetActive(false);
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					if (i >= listParent.childCount)
						listItem = Instantiate(listItemPrefab, listParent);
					else
						listItem = listParent.GetChild(i).GetComponent<ScrollListItem>();

					listItem.gameObject.SetActive(true);

					OnListItemCreate(i, listItem);
					
					if (selectDefaultId != null && selectDefaultId == i)
					{
						listItem.Select();
						Select(i, listItem);
					}

					int id = i;
					ScrollListItem listItemLocal = listItem;
					listItem.selectEntry.callback.RemoveAllListeners();
					listItem.selectEntry.callback.AddListener(delegate { Select(id, listItemLocal); });
					listItem.pointerEntry.callback.RemoveAllListeners();
					listItem.pointerEntry.callback.AddListener(delegate { listItemLocal.Select(); });
					
				}
			}
		}

		public void Select(int id, ScrollListItem listItem)
		{
			selectedId = id;
			updateScrollToTransform = listItem.transform;
			OnListItemSelect?.Invoke(id, listItem);
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			pointerOver = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			pointerOver = false;
		}

		private void OnDisable()
		{
			pointerOver = false;
		}

		private void SelectSelected()
		{
			if(selectedId > -1)
				listParent.GetChild(selectedId).GetComponent<ScrollListItem>().Select();
		}
	}
}