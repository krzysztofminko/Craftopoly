using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBinding : MonoBehaviour
{
	PlayMakerFSM fsm;

	private void Awake()
	{
		fsm = GetComponent<PlayMakerFSM>();
	}


	public string ActiveStateName {	get => fsm.ActiveStateName;	}


	public void GoTo(Vector3 position)
	{
		fsm.FsmVariables.GetFsmVector3("Position").Value = position;
		fsm.SendEvent("GoTo");
	}

	public void Pick(Item item)
	{
		fsm.FsmVariables.GetFsmGameObject("CraftStructureGO").Value = null;
		fsm.FsmVariables.GetFsmGameObject("SourceStorageGO").Value = null;
		fsm.FsmVariables.GetFsmGameObject("ItemGO").Value = item.gameObject;
		fsm.SendEvent("Pick");
	}
	
	public void Pick(Storage sourceStorage, Item item)
	{
		fsm.FsmVariables.GetFsmGameObject("CraftStructureGO").Value = null;
		fsm.FsmVariables.GetFsmGameObject("SourceStorageGO").Value = sourceStorage ? sourceStorage.gameObject : null;
		fsm.FsmVariables.GetFsmGameObject("ItemGO").Value = item.gameObject;
		fsm.SendEvent("Pick");
	}

	public void Put()
	{
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = null;
		fsm.SendEvent("Put");
	}

	public void Put(Storage storage)
	{
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = storage ? storage.gameObject : null;
		fsm.SendEvent("Put");
	}

	public void AttachTool()
	{
		fsm.SendEvent("AttachTool");
	}

	public void DetachTool()
	{
		fsm.SendEvent("DetachTool");
	}

	public void Consume()
	{
		fsm.SendEvent("Consume");
	}

	public void Store(Item item, Storage sourceStorage, Storage storage)
	{
		fsm.FsmVariables.GetFsmGameObject("SourceStorageGO").Value = sourceStorage ? sourceStorage.gameObject : null;
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = storage.gameObject;
		fsm.FsmVariables.GetFsmGameObject("ItemGO").Value = item.gameObject;
		fsm.SendEvent("Store");
	}

	public void Gather(Source source, Storage storage = null)
	{
		fsm.FsmVariables.GetFsmGameObject("SourceGO").Value = source.gameObject;
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = storage ? storage.gameObject : null;
		fsm.SendEvent("Gather");
	}

	public void Craft(ItemType itemType, Structure structure)
	{
		fsm.FsmVariables.GetFsmObject("ItemTypeSO").Value = itemType;
		fsm.FsmVariables.GetFsmGameObject("StructureGO").Value = structure.gameObject;
		fsm.SendEvent("Craft");
	}

	public void Build(Structure structure)
	{
		fsm.FsmVariables.GetFsmGameObject("StructureGO").Value = structure.gameObject;
		fsm.SendEvent("Build");
	}
	
	public void Interrupt()
	{
		fsm.SendEvent("Interrupt");
	}

	public void GetItem(ItemType itemType, int count, Storage storage)
	{
		fsm.FsmVariables.GetFsmObject("ItemTypeSO").Value = itemType;
		fsm.FsmVariables.GetFsmInt("Count").Value = count;
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = storage.gameObject;
		fsm.SendEvent("GetItem");
	}

}
