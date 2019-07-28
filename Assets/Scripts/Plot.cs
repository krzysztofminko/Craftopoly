using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Target))]
public class Plot : MonoBehaviour, IMoney
{
	public static List<Plot> list = new List<Plot>();

	[Header("Runtime")]
	public Vector2Int size = Vector2Int.one;
	public Citizen owner;
	public float value;
	public bool sell;
	public Citizen worker;
	public List<CraftStructure.CraftOrder> orders;
	public Storage sellStorage;
	public float Money { get; set; }

	[Header("References")]
	public Transform[] corners;

	private void Start()
	{
		for (int i = 0; i < corners.Length; i++)
			corners[i].parent = null;
		GetComponent<Collider>().enabled = false;
	}
	/*
	private void Update()
	{
		if (worker && worker.fsm.ActiveStateName == "Idle")
		{
			CraftStructure.CraftOrder order = orders.Find(o => (!o.maintainAmount && o.count > 0) || (o.maintainAmount && o.count > sellStorage.Count(o.itemType)));
			if (order != null)
				worker.fsm.GetItem(order.itemType, order.count, sellStorage);
		}
	}
	*/
	public void Create()
	{
		list.Add(this);
		GetComponent<Collider>().enabled = true;
	}

	public void SetSize(Vector2Int size)
	{
		this.size = new Vector2Int(Mathf.Abs(size.x), Mathf.Abs(size.y));
		transform.localScale = new Vector3(this.size.x, 1, this.size.y);
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
		corners[0].localPosition = position + new Vector3(size.x, 0, size.y) * 0.5f;
		corners[1].localPosition = position + new Vector3(size.x, 0, -size.y) * 0.5f;
		corners[2].localPosition = position + new Vector3(-size.x, 0, size.y) * 0.5f;
		corners[3].localPosition = position + new Vector3(-size.x, 0, -size.y) * 0.5f;
	}

	public void Destroy()
	{
		list.Remove(this);
		for (int i = 0; i < corners.Length; i++)
			Destroy(corners[i].gameObject);
		Destroy(gameObject);
	}

	public void Pay(IMoney payTo, float money)
	{
		Money -= money;
		payTo.Money += money;
	}
}
