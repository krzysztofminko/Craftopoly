using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Target : MonoBehaviour
{
	public static List<Target> list = new List<Target>();

	[Header("Settings")]
	public bool logReservedBy;
	public float hp = 100;
	public float hpMax = 100;
	public float proximity = 1.0f;

	[Header("Runtime")]
	public bool focused;
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

	public void Focus()
	{
		if (!focused)
		{
			focused = true;
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			for (int r = 0; r < renderers.Length; r++)
				for (int m = 0; m < renderers[r].materials.Length; m++)
				{
					Material material = renderers[r].materials[m];
					Color.RGBToHSV(material.color, out float H, out float S, out float V);
					material.color = Color.HSVToRGB(H, S, V + 0.25f);
				}


			if (shopStructure && !ReservedBy)
				shopStructure.StartTransaction(Player.instance);
		}
	}

	public void Unfocus()
	{
		if (focused)
		{
			if (shopStructure && ReservedBy == Player.instance)
				shopStructure.FinishTransaction();

			focused = false;
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			for (int r = 0; r < renderers.Length; r++)
				for (int m = 0; m < renderers[r].materials.Length; m++)
				{
					Material material = renderers[r].materials[m];
					Color.RGBToHSV(material.color, out float H, out float S, out float V);
					material.color = Color.HSVToRGB(H, S, V - 0.25f);
				}
		}
	}

	public bool Damage(float value)
	{
		hp -= value;
		if (hp < 0)
		{
			hp = 0;
			Destroy(gameObject);
			return true;
		}
		return false;
	}

	public bool Repair(float value)
	{
		hp += value;
		if (hp >= hpMax)
		{
			hp = hpMax;			
			return true;
		}
		return false;
	}

	private void OnDestroy()
	{
		if(focused)
			Player.instance.Unfocus();
	}

}
