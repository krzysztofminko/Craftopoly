using System;
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
		public delegate void onListItemCreate(int id, ScrollListItem listItem);
		public event onListItemCreate OnListItemCreate;
		public delegate void onListItemSelect(int id, ScrollListItem listItem);
		public event onListItemSelect OnListItemSelect;

		[HideInInspector]
		public List<ScrollListItem> items = new List<ScrollListItem>();

		[Header("References")]
		[SerializeField]
		private RectTransform listParent;
		[SerializeField]
		private ScrollList leftList;
		[SerializeField]
		private ScrollList rightList;

		private int selectedId = -1;
		private bool pointerOver;
		private ScrollRect scrollRect;
		private ScrollListItem itemPrefab;
		private Transform moveScrollToTransform;


		public void Draw(int count, int? selectDefaultId = null)
		{
			ScrollListItem item;
			for (int i = count; i < listParent.childCount; i++)
				listParent.GetChild(i).gameObject.SetActive(false);
			items.Clear();
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					if (i >= listParent.childCount)
						item = Instantiate(itemPrefab, listParent);
					else
						item = listParent.GetChild(i).GetComponent<ScrollListItem>();

					items.Add(item);
					item.gameObject.SetActive(true);

					OnListItemCreate(i, item);

					if (selectDefaultId != null && selectDefaultId == i)
					{
						item.Select();
						Select(i, item);
					}

					int id = i;
					ScrollListItem listItemLocal = item;
					item.selectEntry.callback.RemoveAllListeners();
					item.selectEntry.callback.AddListener(delegate { Select(id, listItemLocal); });
					item.pointerEntry.callback.RemoveAllListeners();
					item.pointerEntry.callback.AddListener(delegate { listItemLocal.Select(); });
				}
			}
		}

		public void UpdateNavigation()
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].navigation = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnDown = items.Count > 1 ? items[(i == items.Count - 1) ? 0 : i + 1] : null,
					selectOnUp = items.Count > 1 ? items[(i == 0) ? items.Count - 1 : i - 1] : null,
					selectOnLeft = leftList && leftList.items.Count > 0 ? leftList.items[leftList.selectedId > 0 ? leftList.selectedId : 0] : null,
					selectOnRight = rightList && rightList.items.Count > 0 ? rightList.items[rightList.selectedId > 0 ? rightList.selectedId : 0] : null
				};
			}
		}


		private void Awake()
		{
			itemPrefab = listParent.GetChild(0).GetComponent<ScrollListItem>();
			scrollRect = GetComponent<ScrollRect>();
		}

		private void OnEnable()
		{
			scrollRect.enabled = true;
			for (int i = 0; i < items.Count; i++)
				items[i].interactable = true;
		}

		private void OnDisable()
		{
			pointerOver = false;
			scrollRect.enabled = false;
			for (int i = 0; i < items.Count; i++)
				items[i].interactable = false;
		}

		private void Update()
		{
			if(pointerOver)
				scrollRect.verticalNormalizedPosition += Input.mouseScrollDelta.y / listParent.rect.height * scrollRect.scrollSensitivity * 2;

			if (moveScrollToTransform)
			{
				if (moveScrollToTransform.localPosition.y + listParent.localPosition.y > -itemPrefab.GetComponent<RectTransform>().sizeDelta.y)
					scrollRect.verticalNormalizedPosition += Time.deltaTime / listParent.childCount * 20;
				else if (moveScrollToTransform.localPosition.y + listParent.localPosition.y < -scrollRect.GetComponent<RectTransform>().sizeDelta.y + itemPrefab.GetComponent<RectTransform>().sizeDelta.y)
					scrollRect.verticalNormalizedPosition -= Time.deltaTime / listParent.childCount * 20;
				else
					moveScrollToTransform = null;
				if ((selectedId == items.Count - 1 && scrollRect.verticalNormalizedPosition < 0) || (selectedId == 0 && scrollRect.verticalNormalizedPosition > 1))
				{
					moveScrollToTransform = null;
					scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition, 0.0f, 1.0f);
				}
			}
		}

		private void Select(int id, ScrollListItem listItem)
		{
			selectedId = id;
			moveScrollToTransform = listItem.transform;
			OnListItemSelect?.Invoke(id, listItem);
			UpdateNavigation();
			leftList?.UpdateNavigation();
			rightList?.UpdateNavigation();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			pointerOver = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			pointerOver = false;
		}
	}
}