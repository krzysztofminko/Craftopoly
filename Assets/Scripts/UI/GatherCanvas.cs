using System.Collections;
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

		private Canvas canvas;

		private void Awake()
		{
			instance = this;
			canvas = GetComponent<Canvas>();
			canvas.enabled = false;
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

			canvas.enabled = true;

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

			canvas.enabled = false;

			if (Player.instance)
				Player.instance.controlsEnabled = true;
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
				workerNameText.text = $"<i>{Localization.Translate("NO_WORKER_INFO")}</i>";
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