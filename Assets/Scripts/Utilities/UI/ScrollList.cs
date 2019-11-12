﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities.UI
{
	//OPTIONAL: Categories and filters
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollList : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		static List<ScrollList> instances = new List<ScrollList>();

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
		public List<ScrollListItem> listItems = new List<ScrollListItem>();
		int selectedId = -1;
		Transform updateScrollToTransform;
		string hAxis;
		string vAxis;
		bool pointerOver;


		private void Awake()
		{
			instances.Add(this);
			listItemPrefab = listParent.GetChild(0).GetComponent<ScrollListItem>();
			hAxis = EventSystem.current.GetComponent<StandaloneInputModule>().horizontalAxis;
			vAxis = EventSystem.current.GetComponent<StandaloneInputModule>().verticalAxis;
			scrollRect = GetComponent<ScrollRect>();
			scrollRect.verticalScrollbar.onValueChanged.AddListener(delegate { SelectSelected(); });
		}

		private void OnDestroy()
		{
			instances.Remove(this);
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
			listItems = new List<ScrollListItem>();
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					if (i >= listParent.childCount)
						listItem = Instantiate(listItemPrefab, listParent);
					else
						listItem = listParent.GetChild(i).GetComponent<ScrollListItem>();

					listItems.Add(listItem);
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
			UpdateNavigation();
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

		private ScrollList GetListOnLeft()
		{
			float dist = Mathf.Infinity;
			ScrollList leftList = null;
			for (int i = 0; i < instances.Count; i++)
				if (instances[i] != this && instances[i].gameObject.activeInHierarchy)
					if (instances[i].transform.position.x < transform.position.x)
						if (transform.position.x - instances[i].transform.position.x < dist)
						{
							leftList = instances[i];
							dist = transform.position.x - instances[i].transform.position.x;
						}
			return leftList;
		}

		private ScrollList GetListOnRight()
		{
			float dist = Mathf.Infinity;
			ScrollList rightList = null;
			for (int i = 0; i < instances.Count; i++)
				if (instances[i] != this && instances[i].gameObject.activeInHierarchy)
					if (instances[i].transform.position.x > transform.position.x)
						if (instances[i].transform.position.x - transform.position.x < dist)
						{
							rightList = instances[i];
							dist = instances[i].transform.position.x - transform.position.x;
						}
			return rightList;
		}

		public static void UpdateNavigation()
		{
			for (int l = 0; l < instances.Count; l++)
			{
				ScrollList leftList = instances[l].GetListOnLeft();
				ScrollList rightList = instances[l].GetListOnRight();

				for (int i = 0; i < instances[l].listItems.Count; i++)
				{

					instances[l].listItems[i].navigation = new Navigation
					{
						mode = Navigation.Mode.Explicit,
						selectOnDown = instances[l].listItems.Count > 1 ? instances[l].listItems[(i == instances[l].listItems.Count - 1) ? 0 : i + 1] : null,
						selectOnUp = instances[l].listItems.Count > 1 ? instances[l].listItems[(i == 0) ? instances[l].listItems.Count - 1 : i - 1] : null,
						selectOnLeft = leftList && leftList.listItems.Count > 0 ? leftList.listItems[leftList.selectedId > 0 ? leftList.selectedId : 0] : null,
						selectOnRight = rightList && rightList.listItems.Count > 0 ? rightList.listItems[rightList.selectedId > 0 ? rightList.selectedId: 0] : null
					};
				}
			}
		}
	}
}