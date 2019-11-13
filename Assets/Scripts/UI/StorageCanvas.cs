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
		private List<ItemType> itemTypes;

		private Canvas canvas;

		private void Awake()
		{
			instance = this;

			targetStorageScrollList.OnListItemCreate += SetupItem;
			targetStorageScrollList.OnListItemSelect += SelectItem;

			canvas = GetComponent<Canvas>();
			Hide();
		}

		private void OnDestroy()
		{
			Hide();
			targetStorageScrollList.OnListItemCreate -= SetupItem;
			targetStorageScrollList.OnListItemSelect -= SelectItem;
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

				Player.instance.controlsEnabled = false;

				UpdateList();

				detailsNameText.text = "";
				detailsImage.sprite = null;
				detailsDescriptionText.text = "";

				UpdatePickButton();

				targetStorage.onItemsUpdate += UpdateList;

				controlsUnlocked = false;

				canvas.enabled = true;
				targetStorageScrollList.enabled = true;
			}
		}

		public void Hide()
		{
			if (targetStorage)
			{
				targetStorage.onItemsUpdate -= UpdateList;
				targetStorage = null;

				selectedItem = null;
				targetStorage = null;
				selectedListItem = null;

				if (Player.instance)
					Player.instance.controlsEnabled = true;
			}
			canvas.enabled = false;
			targetStorageScrollList.enabled = false;
		}

		
		private void SetupItem(int id, Selectable listItem)
		{
			listItem.transform.GetChild(0).GetComponent<Image>().sprite = itemTypes[id].GenerateThumbnail();
			listItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemTypes[id].name;
			listItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = targetStorage.Count(itemTypes[id]).ToString();
		}

		private void SelectItem(int id, Selectable listItem)
		{
			selectedListItemId = id;
			selectedListItem = listItem;
			selectedItem = targetStorage.items.Find(i => i.type == itemTypes[id]);

			UpdateDetails();
		}

		private void UpdateList()
		{
			itemTypes = new List<ItemType>();
			for (int i = 0; i < targetStorage.items.Count; i++)
				if (!itemTypes.Contains(targetStorage.items[i].type))
					itemTypes.Add(targetStorage.items[i].type);

			targetStorageScrollList.Draw(itemTypes.Count, 0);
			targetStorageScrollList.UpdateNavigation();
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