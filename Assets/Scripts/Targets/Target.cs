using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Target : MonoBehaviour
{
	public static List<Target> list = new List<Target>();
	
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
	
}
