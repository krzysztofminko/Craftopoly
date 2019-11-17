using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Profiling;

namespace UI
{
	public class WorkplaceCanvas : MonoBehaviour
	{
		public static WorkplaceCanvas instance;

		public bool visible;
		public Citizen selectedCitizen;
		public Workplace selectedWorkplace;

		[Header("References")]
		public ScrollList citizensScrollList;
		public ScrollList workplaceScrollList;
		public TextMeshProUGUI detailsNameText;
		public Image detailsImage;
		public Image workplaceImage;
		public TextMeshProUGUI workplaceNameText;
		public Button workplaceDismissButton;
		public RectTransform skillList;

		private List<Citizen> citizens;
		private List<Workplace> workplaces;

		private bool controlsUnlocked;

		private Canvas canvas;

		private void Awake()
		{
			instance = this;
			canvas = GetComponent<Canvas>();
			Hide();
		}

		private void Update()
		{
			if (visible && controlsUnlocked)
			{
				if (Input.GetButtonDown("Cancel"))
					Hide();

				if (Input.GetButtonDown("DismissWork"))
					DismissWorkplace();

				if (Input.GetButtonDown("Submit"))
					AssignWorkplace();
			}
			else
			{
				controlsUnlocked = true;
			}
		}


		public void Show()
		{
			visible = true;
			
			canvas.enabled = true;

			citizens = Citizen.list;
			workplaces = Workplace.list;

			citizensScrollList.OnListItemCreate += SetupCitizenListItem;
			citizensScrollList.OnListItemSelect += SelectCitizenListItem;

			workplaceScrollList.OnListItemCreate += SetupWorkplaceListItem;
			workplaceScrollList.OnListItemSelect += SelectWorkplaceListItem;

			citizensScrollList.enabled = true;
			workplaceScrollList.enabled = true;

			citizensScrollList.Draw(citizens.Count, 0);
			workplaceScrollList.Draw(workplaces.Count);

			citizensScrollList.UpdateNavigation();
			workplaceScrollList.UpdateNavigation();

			Player.instance.controlsEnabled = false;
		}

		public void Hide()
		{
			visible = false;

			canvas.enabled = false;
			
			citizensScrollList.OnListItemCreate -= SetupCitizenListItem;
			citizensScrollList.OnListItemSelect -= SelectCitizenListItem;

			workplaceScrollList.OnListItemCreate -= SetupWorkplaceListItem;
			workplaceScrollList.OnListItemSelect -= SelectWorkplaceListItem;

			citizensScrollList.enabled = false;
			workplaceScrollList.enabled = false;

			Player.instance.controlsEnabled = true;
		}


		private void SetupCitizenListItem(int id, Selectable listItem)
		{
			//listItem.transform.GetChild(0).GetComponent<Image>().sprite = Thumbnail.Generate(citizens[id].transform);
			listItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = citizens[id].name;
		}

		private void SelectCitizenListItem(int id, Selectable listItem)
		{
			selectedCitizen = citizens[id];
			UpdateDetails();
		}

		private void SetupWorkplaceListItem(int id, Selectable listItem)
		{
			//listItem.transform.GetChild(0).GetComponent<Image>().sprite = Thumbnail.Generate(workplaces[id].transform);
			listItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = workplaces[id].name;
		}

		private void SelectWorkplaceListItem(int id, Selectable listItem)
		{
			selectedWorkplace = workplaces[id];
		}

		private void UpdateDetails()
		{
			detailsNameText.text = selectedCitizen.name;
			detailsImage.sprite = Thumbnail.Generate(selectedCitizen.transform);

			var skills = selectedCitizen.skills.GetAll();
			for (int i = 0; i < Mathf.Max(skills.Count, skillList.childCount); i++)
			{
				if (i < skills.Count && i < skillList.childCount)
				{
					skillList.GetChild(i).gameObject.SetActive(true);
					skillList.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = skills[i].name.ToString();
					skillList.GetChild(i).GetChild(1).GetComponent<Slider>().value = skills[i].value;
				}
				else
				{
					skillList.GetChild(i).gameObject.SetActive(false);
				}
			}

			UpdateWorkplacePanel();
		}

		private void UpdateWorkplacePanel()
		{
			if (selectedCitizen.workplace)
			{
				workplaceImage.sprite = Thumbnail.Generate(selectedCitizen.workplace.transform);
				workplaceNameText.text = selectedCitizen.workplace.name;
				workplaceDismissButton.interactable = true;
			}
			else
			{
				workplaceImage.sprite = null;
				workplaceNameText.text = "<i>No workplace.</i>";
				workplaceDismissButton.interactable = false;
			}
		}
		

		public void AssignWorkplace() //Called by AssignButton
		{
			if (selectedWorkplace)
			{
				selectedCitizen.AssignWorkplace(selectedWorkplace);
				UpdateWorkplacePanel();
			}
		}

		public void DismissWorkplace() //Called by DismissButton
		{
			if (selectedCitizen.workplace)
			{
				MessageForm.instance.Show("Do you really want to fire this worker?", new Dictionary<string, UnityAction>
				{
					{"Yes", delegate { selectedCitizen.AssignWorkplace(null); UpdateWorkplacePanel(); } },
					{"No", null }
				}, transform.GetChild(0).GetComponent<RectTransform>());
			}
		}

	}
}