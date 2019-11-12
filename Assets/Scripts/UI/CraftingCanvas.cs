using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.UI;
using UnityEngine.Events;
using static Localization;
using System;

namespace UI
{
	public class CraftingCanvas : MonoBehaviour
	{
		public static CraftingCanvas instance;

		public CraftStructure structure;
		public ItemType itemType;
		public CraftStructure.CraftOrder order;
		public Selectable selectedListItem;

		[Header("References")]
		public Image detailsImage;
		public TextMeshProUGUI productNameText;
		public TextMeshProUGUI descriptionText;
		public Image workerImage;
		public TextMeshProUGUI workerNameText;
		public Button workerDismissButton;
		public TextMeshProUGUI orderCountText;
		public Button incButton;
		public Button decButton;
		public Toggle maintainAmountToggle;
		public ScrollList productsScrollList;
		public Transform itemList;
		public Scrollbar itemScrollbar;
		public Transform itemPanel;
		public Button craftButton;
		public Slider fuelSlider;
		public TextMeshProUGUI fuelText;
		public Slider progressSlider;

		int selectedProductId;
		private bool controlsUnlocked;

		private void Awake()
		{
			instance = this;

			productsScrollList.OnListItemCreate += SetupProductListItem;
			productsScrollList.OnListItemSelect += SelectProductListItem;

			transform.GetChild(0).gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			Hide();
			productsScrollList.OnListItemCreate -= SetupProductListItem;
			productsScrollList.OnListItemSelect -= SelectProductListItem;
		}

		private void Update()
		{
			if (structure && controlsUnlocked)
			{
				progressSlider.value = structure.progress;
				fuelSlider.value = structure.fuel / structure.fuelMax;
				TimeSpan ts = TimeSpan.FromSeconds(structure.fuel);
				fuelText.text = "Fuel for " + ((int)ts.TotalHours > 0? (int)ts.TotalHours + "h " : "") + ((int)ts.Minutes > 0 ? (int)ts.TotalMinutes + "m " : "") + (int)ts.Seconds + "s";
				//fuelText.text = "Fuel for " + (structure.fuel >= 60 ? (int)(structure.fuel / 60) + "min " + (structure.fuel % 60) : structure.fuel + "s");

				if (Input.GetButtonDown("Submit"))
					SetCurrentBlueprint();

				if (Input.GetButtonDown("DismissWork"))
					DismissWorker();

				if (Input.GetButtonDown("MaintainCount"))
					maintainAmountToggle.isOn = !maintainAmountToggle.isOn;

				if (Input.GetButtonDown("IncCount"))
					IncOrderCount();

				if (Input.GetButtonDown("DecCount"))
					DecOrderCount();

				if (Input.GetButtonDown("Cancel"))
					Hide();
			}
			else
			{
				controlsUnlocked = true;
			}
		}


		public void Show(CraftStructure structure)
		{
			if (!this.structure)
			{
				this.structure = structure;
				structure.ReservedBy = Player.instance;
				transform.GetChild(0).gameObject.SetActive(true);

				Player.instance.controlsEnabled = false;

				productsScrollList.Draw(structure.itemTypes.Count, 0);
				ScrollList.UpdateNavigation();

				structure.storage.onItemsUpdate += UpdateRequiredList;

				fuelSlider.gameObject.SetActive(structure.fuelMax > 0);
				progressSlider.gameObject.SetActive(structure.fuelMax > 0);
				progressSlider.value = 0;

				controlsUnlocked = false;
			}
		}

		public void Hide(bool freeReservation = true)
		{
			if (structure)
			{
				if (freeReservation && structure)
					structure.ReservedBy = null;

				structure.storage.onItemsUpdate -= UpdateRequiredList;

				structure = null;
				itemType = null;
				transform.GetChild(0).gameObject.SetActive(false);


				if (Player.instance)
					Player.instance.controlsEnabled = true;
			}
		}


		private void SetupProductListItem(int id, Selectable listItem)
		{
			listItem.transform.GetChild(0).GetComponent<Image>().sprite = structure.itemTypes[id].GenerateThumbnail();
			listItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = structure.itemTypes[id].name;
			SetListItemCountText(listItem, structure.orders[id].count);
		}

		private void SelectProductListItem(int id, Selectable listItem)
		{
			selectedProductId = id;
			selectedListItem = listItem;
			itemType = structure.itemTypes[id];
			order = structure.orders.Find(o => o.itemType == itemType);
			UpdateCraftButton();
			UpdateDetails();
		}

		private void UpdateCraftButton()
		{
			craftButton.interactable = true; // itemType.blueprint.MissingResources(structure.storage).Count == 0;
		}

		private void UpdateDetails()
		{
			detailsImage.sprite = itemType.GenerateThumbnail();
			productNameText.text = itemType.name;
			descriptionText.text = itemType.description;
			UpdateWorkerPanel();
			orderCountText.text = order.count.ToString();
			maintainAmountToggle.isOn = order.maintainAmount;

			UpdateRequiredList();
		}

		private void UpdateRequiredList()
		{
			for (int i = itemType.blueprint.requiredItems.Count; i < itemList.childCount; i++)
				itemList.GetChild(i).gameObject.SetActive(false);
			for (int i = 0; i < itemType.blueprint.requiredItems.Count; i++)
			{
				if (i >= itemList.childCount)
					itemPanel = Instantiate(itemList.GetChild(0), itemList);
				else
					itemPanel = itemList.GetChild(i);

				itemPanel.gameObject.SetActive(true);
				itemPanel.GetChild(0).GetComponent<Image>().sprite = itemType.blueprint.requiredItems[i].type.GenerateThumbnail();
				itemPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = itemType.blueprint.requiredItems[i].type.name;
				itemPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = structure.storage.Count(itemType.blueprint.requiredItems[i].type).ToString() + " / " + itemType.blueprint.requiredItems[i].count.ToString();
			}

			UpdateCraftButton();
		}

		private void SetListItemCountText(Selectable listItem, int count)
		{
			listItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = count > 0 ? "x" + count : "";
		}

		private void UpdateWorkerPanel()
		{
			if (structure.worker)
			{
				workerImage.sprite = Thumbnail.Generate(structure.worker.transform);
				workerNameText.text = structure.worker.name;
				workerDismissButton.interactable = true;
			}
			else
			{
				workerImage.sprite = null;
				workerNameText.text = "<i>" + Translate("NO_WORKER_INFO") + "</i>";
				workerDismissButton.interactable = false;
			}
		}


		public void DismissWorker() //Called by DismissButton
		{
			MessageForm.instance.Show("Do you really want to fire this worker?", new Dictionary<string, UnityAction>
			{
				{"Yes", delegate { structure.worker.AssignWorkplace(null); UpdateWorkerPanel(); } },
				{"No", null }
			}, transform.GetChild(0).GetComponent<RectTransform>());
		}

		public void IncOrderCount() //Called by IncButton
		{
			order.count = Mathf.Min(9999, order.count + 1);
			orderCountText.text = order.count.ToString();
			SetListItemCountText(selectedListItem, order.count);
		}

		public void DecOrderCount() //Called by DecButton
		{
			order.count = Mathf.Max(0, order.count - 1);
			orderCountText.text = order.count.ToString();
			SetListItemCountText(selectedListItem, order.count);
		}

		public void SetMaintainAmount() //Called by MaintainToggle
		{
			order.maintainAmount = maintainAmountToggle.isOn;
		}

		public void SetCurrentBlueprint() //Called by CraftButton
		{
			if (craftButton.interactable)
			{
				structure.currentItemType = itemType;
				if(structure.fuelMax == 0)
					Hide(false);
			}
		}

	}
}