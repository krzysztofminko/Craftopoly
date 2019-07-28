using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class NewPlotCanvas : MonoBehaviour
	{
		public static NewPlotCanvas instance;

		public static Plot createNewPlot;

		public bool holdToCreate;

		[Header("References")]
		public Button newPlotButton;
		public Plot plotPrefab;

		Vector3? newPlotStart;
		RaycastHit hitInfo;
		Vector3 cursorPosition = new Vector3(Screen.width, Screen.height, 0) * 0.5f;

		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
			if (createNewPlot)
				NewPlotCreation();			
		}

		public void Show(bool toggle)
		{
			transform.GetChild(0).gameObject.SetActive(toggle);
		}

		public void NewPlot()   //Called by newPlotButton OnClick
		{
			if (!createNewPlot)
			{
				createNewPlot = Instantiate(plotPrefab);
				createNewPlot.name = plotPrefab.name;
				createNewPlot.owner = Player.instance;
				newPlotStart = null;
				newPlotButton.gameObject.SetActive(false);
			}
		}

		public void NewPlotCreation()
		{
			if (Input.GetButtonDown("SecondaryAction") || Input.GetButtonDown("Cancel"))
			{
				createNewPlot.Destroy();
				createNewPlot = null;
				newPlotButton.gameObject.SetActive(true);
			}
			else
			{
				if(Game.controller)
					Physics.Raycast(Camera.main.ScreenPointToRay(cursorPosition += new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0) * 4), out hitInfo, 100, LayerMask.GetMask("Terrain"));
				else
					Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, LayerMask.GetMask("Terrain"));
				if (newPlotStart == null)
				{
					createNewPlot.SetPosition(hitInfo.point + new Vector3(createNewPlot.size.x, 0, createNewPlot.size.y) * 0.5f);
					if (Input.GetButtonDown("PrimaryAction"))
						newPlotStart = hitInfo.point;
				}
				else
				{
					Vector3 size = hitInfo.point - newPlotStart.Value;
					//createNewPlot.size = new Vector2Int((int)Mathf.Max(1, size.x), (int)Mathf.Max(1, size.z));
					//createNewPlot.transform.localScale = new Vector3(createNewPlot.size.x, 1, createNewPlot.size.y);
					createNewPlot.SetSize(new Vector2Int((int)size.x, (int)size.z));
					createNewPlot.SetPosition(newPlotStart.Value + new Vector3((int)size.x, 0, (int)size.z) * 0.5f);
					if ((Input.GetButtonDown("PrimaryAction") && !holdToCreate) || (Input.GetButtonDown("PrimaryAction") && holdToCreate))
					{
						createNewPlot.Create();
						createNewPlot = null;
						newPlotButton.gameObject.SetActive(true);
						cursorPosition = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
					}
				}
			}
		}
	}
}