using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEditor;

public class Game : MonoBehaviour {

	public static Game instance;

	public static bool controller;
	[SerializeField]
	private bool _controller;

	public NewStructure createNewStructure;
	
	[Header("References")]
	public LayerMask selectionLayerMask;
	public NewStructure newStructurePrefab;

	RaycastHit hitInfo;
	Plot newStructurePlot;

	private void Awake()
	{
		instance = this;
	}

	void Update () {

		if (!UI.NewPlotCanvas.createNewPlot && createNewStructure)
			NewStructureCreation();

		string[] joys = Input.GetJoystickNames() ;
		controller = joys.Length > 0 && joys.Any(j => !string.IsNullOrEmpty(j));
		_controller = controller;
	}

	//TODO: NewStructureCreation - Move to Player
	void NewStructureCreation()
	{
		if (!newStructurePlot && Player.instance.focusedOn && Player.instance.focusedOn.GetComponent<Plot>())
		{
			newStructurePlot = Player.instance.focusedOn.GetComponent<Plot>();
			return;
		}

		if (Input.GetAxis("Horizontal2") != 0 || Input.GetAxis("Vertical2") != 0)
		{
			Vector3 newPosition = createNewStructure.transform.position + new Vector3(Input.GetAxis("Horizontal2") * 10 * Time.deltaTime, 0, Input.GetAxis("Vertical2") * 10 * Time.deltaTime);
			newPosition = new Vector3(Mathf.Clamp(newPosition.x, newStructurePlot.transform.position.x - newStructurePlot.transform.localScale.x * 0.5f, newStructurePlot.transform.position.x + newStructurePlot.transform.localScale.x * 0.5f), newPosition.y, Mathf.Clamp(newPosition.z, newStructurePlot.transform.position.z - newStructurePlot.transform.localScale.z * 0.5f, newStructurePlot.transform.position.z + newStructurePlot.transform.localScale.z * 0.5f));
			createNewStructure.transform.position = newPosition;
		}
		else if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, LayerMask.GetMask("Terrain"))
				&& new Rect(newStructurePlot.transform.position.x - newStructurePlot.transform.localScale.x * 0.5f, newStructurePlot.transform.position.z - newStructurePlot.transform.localScale.z * 0.5f, newStructurePlot.transform.localScale.x, newStructurePlot.transform.localScale.z).Contains(new Vector2(hitInfo.point.x, hitInfo.point.z)))
				createNewStructure.transform.position = hitInfo.point;
			else
				createNewStructure.transform.position = newStructurePlot.transform.position;
		}

		if (Input.GetButtonDown("PrimaryAction"))
		{
			createNewStructure.Create();
			createNewStructure = null;
			newStructurePlot = null;
		}

		if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("SecondaryAction"))
		{
			Destroy(createNewStructure.gameObject);
			createNewStructure = null;
			newStructurePlot = null;
		}
	}



	public static GameObject Spawn(GameObject prefab, Vector3? position = null, Quaternion? rotation = null)
	{
#if UNITY_EDITOR
		if (prefab.GetComponent<Item>())
			Debug.LogError("Items must be spawned by ItemType not Game class.");
#endif

		GameObject gameObject = Instantiate(prefab, position ?? Vector3.zero, rotation ?? Quaternion.identity);

		ISpawnable[] spawnables = gameObject.GetComponents<ISpawnable>();
		for(int i = 0; i < spawnables.Length; i++)
			spawnables[i].OnSpawn();

		return gameObject;
	}

	public static void Depawn(GameObject gameObject)
	{
		ISpawnable[] spawnables = gameObject.GetComponents<ISpawnable>();
		for (int i = 0; i < spawnables.Length; i++)
			spawnables[i].OnDespawn();

		Destroy(gameObject);
	}

}
