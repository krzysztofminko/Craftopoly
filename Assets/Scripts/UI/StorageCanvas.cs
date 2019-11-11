using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.UI;

namespace UI
{
	public class StorageCanvas : MonoBehaviour
	{
		public static StorageCanvas instance;

		public Storage targetStorage;
		public ShopStructure shopStructure;
		public int selectedListItemId;
		public Selectable selectedListItem;
		public Item selectedItem;

		[Header("References")]
		public ScrollList targetStorageScrollList;
		public TextMeshProUGUI detailsNameText;
		public Image detailsImage;
		public TextMeshProUGUI detailsDescriptionText;
		public Button incButton;
		public Button decButton;
		public Button pickButton;
		public TextMeshProUGUI pickButtonText;
		public TextMeshProUGUI costText;
		public TextMeshProUGUI moneyText;

		private int pickCount;
		private bool controlsUnlocked;

		private void Awake()
		{
			instance = this;

			targetStorageScrollList.OnListItemCreate += SetupItemListItem;
			targetStorageScrollList.OnListItemSelect += SelectItemListItem;

			transform.GetChild(0).gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			Hide();
			targetStorageScrollList.OnListItemCreate -= SetupItemListItem;
			targetStorageScrollList.OnListItemSelect -= SelectItemListItem;
		}

		private void Update()
		{
			if (targetStorage && shopStructure)
				moneyText.text = "Your money: " + Player.instance.Money.ToString("0.00");

			if (targetStorage && controlsUnlocked)
			{
				if (Input.GetButtonDown("Cancel"))
					Hide();

				if (Input.GetButtonDown("Submit"))
					Pick();

				if (Input.GetButtonDown("IncCount"))
					IncCount();

				if (Input.GetButtonDown("DecCount"))
					DecCount();
			}
			else
			{
				controlsUnlocked = true;
			}
		}


		public void Show(Storage storage, ShopStructure shopStructure = null)
		{
			if (!targetStorage)
			{
				this.shopStructure = shopStructure;
				moneyText.gameObject.SetActive(shopStructure);
				costText.gameObject.SetActive(shopStructure);
				targetStorage = storage;
				transform.GetChild(0).gameObject.SetActive(true);

				Player.instance.controlsEnabled = false;
				
				targetStorageScrollList.Draw(storage.items.Count, 0);

				detailsNameText.text = "";
				detailsImage.sprite = null;
				detailsDescriptionText.text = "";

				UpdatePickButton();

				targetStorage.onItemsUpdate += UpdateItems;

				controlsUnlocked = false;
			}
		}

		public void Hide()
		{
			if (targetStorage)
			{
				targetStorage.onItemsUpdate -= UpdateItems;
				targetStorage = null;
				transform.GetChild(0).gameObject.SetActive(false);

				selectedItem = null;
				targetStorage = null;
				selectedListItem = null;

				if (Player.instance)
					Player.instance.controlsEnabled = true;
			}
		}

		
		private void SetupItemListItem(int id, Selectable listItem)
		{
			listItem.transform.GetChild(0).GetComponent<Image>().sprite = targetStorage.items[id].type.GenerateThumbnail();
			listItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = targetStorage.items[id].type.name;
			SetListItemCountText(listItem, targetStorage.Count(targetStorage.items[id].type));
		}

		private void SelectItemListItem(int id, Selectable listItem)
		{
			selectedListItemId = id;
			selectedListItem = listItem;
			selectedItem = targetStorage.items[id];

			UpdateDetails();
		}

		private void UpdateItems()
		{
			targetStorageScrollList.Draw(targetStorage.items.Count, 0);
		}

		private void UpdateDetails()
		{
			detailsNameText.text = selectedItem.type.name;
			detailsImage.sprite = selectedItem.type.GenerateThumbnail();
			detailsDescriptionText.text = selectedItem.type.description;

			//pickCount = Mathf.Min(selectedItem.type.maxCount, selectedItem.count);
			pickCount = selectedItem.type.maxCount;

			UpdatePickButton();
		}

		private void UpdatePickButton()
		{
			pickButtonText.text = (shopStructure ? "Buy" : "Pick") + " x" + pickCount.ToString();

			pickButton.interactable = targetStorage.items.Count > 0;
			incButton.interactable = targetStorage.items.Count > 0;
			decButton.interactable = targetStorage.items.Count > 0;

			if (shopStructure)
				costText.text = "Cost: " + (selectedItem? (selectedItem.type.value * pickCount).ToString("0.00") : "0");
		}

		private void SetListItemCountText(Selectable listItem, int count)
		{
			listItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = count.ToString();
		}


		public void IncCount() //Called by IncButton
		{
			pickCount = Mathf.Min(targetStorage.Count(selectedItem.type), pickCount + 1);
			UpdatePickButton();
		}

		public void DecCount() //Called by DecButton
		{
			pickCount = Mathf.Max(1, pickCount - 1);
			UpdatePickButton();
		}

		public void Pick() //Called by PickButton
		{
			if (selectedItem)
			{
				if (pickCount > selectedItem.type.maxCount)
				{
					Notifications.instance.Add("Too much.");
				}
				else if (shopStructure && Player.instance.Money < selectedItem.type.value * pickCount)
				{
					Notifications.instance.Add("Not enough money.");
				}
				else
				{
					Player.instance.fsm.Pick(targetStorage, selectedItem);
					Hide();
				}
			}
		}

	}
}