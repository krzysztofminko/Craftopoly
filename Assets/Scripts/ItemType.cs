using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Definition of Item type, sharing all variables in ScriptableObject.
/// </summary>

[CreateAssetMenu(fileName = "New Item Type", menuName = "Item Type")]
public class ItemType : ScriptableObject
{
	public enum PickParent { Front, HandR, HandL }
	public enum AttachParent { None, BeltLeft }

	public GameObject model;
	public Sprite thumbnail;
	[Tooltip("Max count possible to pick")]
	public int maxCount = 1;
	[Tooltip("Must be greater than sum of blueprint items values")]
	public float value;
	[Tooltip("Skill required for crafting or gathering.")]
	public Skills.SkillValue requiredSkill;
	public PickParent pickParent;
	[TextArea]
	public string description = "Description...";
	public Blueprint blueprint;

	[Header("Tool")]
	[Tooltip("None when not tool")]
	public AttachParent attachParent;
	public float damage;
	public float maxDurability;

	[Header("Consumable")]
	[Tooltip("0 when not consumable")]
	public float consumableValue;

	[Header("Fuel")]
	[Tooltip("0 when not fuel. In seconds of burning.")]
	public float fuelValue;


	Texture2D thumbnailTexture;

	public Sprite GenerateThumbnail()
	{
		if (thumbnail)
			return thumbnail;
		//TODO: Change GenerateThumbnail to handmaded thumbnails.
		//This method is generating separate textures and sprites for every instance and every canvas updates!

		//RuntimePreviewGenerator.BackgroundColor = Color.white;
		RuntimePreviewGenerator.Padding = 0.25f;
		RuntimePreviewGenerator.OrthographicMode = true;
		thumbnailTexture = RuntimePreviewGenerator.GenerateModelPreview(model.transform, 512, 512, false);
		thumbnail = Sprite.Create(thumbnailTexture, new Rect(0, 0, thumbnailTexture.width, thumbnailTexture.height), new Vector2(0.5f, 0.5f));
		return thumbnail;
	}
	
	public Item Spawn(int count = 1, Vector3? position = null, Quaternion? rotation = null)
	{
		GameObject go = Instantiate(Resources.Load<GameObject>("Item"));
		if (position != null)
			go.transform.position = position.Value + Vector3.up * 0.5f;
		if (rotation != null)
			go.transform.rotation = rotation.Value;

		Item item = go.GetComponent<Item>();
		item.name = name;
		item.type = this;
		item.count = count;
		item.durability = maxDurability;
		item.SetParent(null);

		GameObject child = Instantiate(model, item.transform);
		child.transform.localPosition = Vector3.zero;

		return item;
	}
}
