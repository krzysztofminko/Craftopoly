using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		public Button pickButton;
		public TextMeshProUGUI pickButtonText;
		public TextMeshProUGUI costText;
		public TextMeshProUGUI moneyText;

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
				
				targetStorageScrollList.Draw(storage.counts.Count, 0);

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
			listItem.transform.GetChild(0).GetComponent<Image>().sprite = targetStorage.counts.ToList()[id].Key.GenerateThumbnail();
			listItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = targetStorage.counts.ToList()[id].Key.name;
			SetListItemCountText(listItem, targetStorage.counts.ToList()[id].Value);
		}

		private void SelectItemListItem(int id, Selectable listItem)
		{
			selectedListItemId = id;
			selectedListItem = listItem;
			selectedItem = targetStorage.items.Find(i => i.type == targetStorage.counts.ToList()[id].Key);

			UpdateDetails();
		}

		private void UpdateItems()
		{
			targetStorageScrollList.Draw(targetStorage.counts.Count, 0);
		}

		private void UpdateDetails()
		{
			detailsNameText.text = selectedItem.type.name;
			detailsImage.sprite = selectedItem.type.GenerateThumbnail();
			detailsDescriptionText.text = selectedItem.type.description;
			
			UpdatePickButton();
		}

		private void UpdatePickButton()
		{
			pickButtonText.text = shopStructure ? "Buy" : "Pick";

			pickButton.interactable = targetStorage.items.Count > 0;

			if (shopStructure)
				costText.text = "Cost: " + (selectedItem? selectedItem.type.value.ToString("0.00") : "0");
		}

		private void SetListItemCountText(Selectable listItem, int count)
		{
			listItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = count.ToString();
		}
		
		public void Pick() //Called by PickButton
		{
			if (selectedItem)
			{
				if (shopStructure && Player.instance.Money < selectedItem.type.value)
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