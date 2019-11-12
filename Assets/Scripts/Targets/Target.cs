using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Target : MonoBehaviour
{
	public static List<Target> list = new List<Target>();

	[Header("Settings")]
	public bool logReservedBy;
	public float proximity = 1.0f;

	[Header("Runtime")]
	[SerializeField]
	private Citizen _reservedBy;
	public Citizen ReservedBy
	{
		get => _reservedBy;
		set
		{
			if (logReservedBy)
				Debug.Log(this + " reserved by: " + value, this);
			_reservedBy = value;
		}
	}

	[Header("Components")]
	public Citizen citizen;
	public Structure structure;
	public Source source;
	public Item item;
	public Storage storage;
	public House house;
	public Plot plot;
	public NewStructure newStructure;
	public CraftStructure craftStructure;
	public GatherStructure gatherStructure;
	public ShopStructure shopStructure;
	public Workplace workplace;


	private void Awake()
	{
		list.Add(this);

		citizen = GetComponent<Citizen>();
		structure = GetComponent<Structure>();
		source = GetComponent<Source>();
		item = GetComponent<Item>();
		storage = GetComponent<Storage>();
		house = GetComponent<House>();
		plot = GetComponent<Plot>();
		newStructure = GetComponent<NewStructure>();
		craftStructure = GetComponent<CraftStructure>();
		workplace = GetComponent<Workplace>();
		gatherStructure = GetComponent<GatherStructure>();
		shopStructure = GetComponent<ShopStructure>();
	}

	public Sprite GenerateThumbnail()
	{
		//RuntimePreviewGenerator.BackgroundColor = Color.white;
		RuntimePreviewGenerator.Padding = 0.25f;
		RuntimePreviewGenerator.OrthographicMode = true;
		Texture2D thumbnail = RuntimePreviewGenerator.GenerateModelPreview(transform, 512, 512, false);
		return Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), new Vector2(0.5f, 0.5f));
	}

}
