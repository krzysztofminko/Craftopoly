﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.UI;

namespace UI
{
	public class GatherCanvas : MonoBehaviour
	{
		public static GatherCanvas instance;

		public GatherStructure structure;

		[Header("References")]
		public TextMeshProUGUI itemNameText;
		public Image itemImage;
		public TextMeshProUGUI itemDescriptionText;
		public Image workerImage;
		public TextMeshProUGUI workerNameText;
		public Button workerDismissButton;

		private bool controlsUnlocked;


		private void Awake()
		{
			instance = this;
			transform.GetChild(0).gameObject.SetActive(false);
		}

		private void Update()
		{
			if (structure && controlsUnlocked)
			{
				if (Input.GetButtonDown("Cancel"))
					Hide();

				if (Input.GetButtonDown("DismissWork"))
					DismissWorker();
			}
			else
			{
				controlsUnlocked = true;
			}
		}


		public void Show(GatherStructure structure)
		{
			this.structure = structure;

			transform.GetChild(0).gameObject.SetActive(true);

			itemNameText.text = structure.itemType.name;
			itemImage.sprite = structure.itemType.GenerateThumbnail();
			itemDescriptionText.text = structure.itemType.description;

			UpdateWorkerPanel();

			controlsUnlocked = false;

			Player.instance.controlsEnabled = false;
		}

		public void Hide()
		{
			structure = null;

			transform.GetChild(0).gameObject.SetActive(false);

			if (Player.instance)
				Player.instance.controlsEnabled = true;
		}


		private void UpdateWorkerPanel()
		{
			if (structure.worker)
			{
				workerImage.sprite = structure.worker.target.GenerateThumbnail();
				workerNameText.text = structure.worker.name;
				workerDismissButton.interactable = true;
			}
			else
			{
				workerImage.sprite = null;
				workerNameText.text = "<i>No worker employed.</i>";
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

	}
}