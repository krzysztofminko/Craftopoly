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

	public void Pick(Storage sourceStorage, Item item, int count)
	{
		fsm.FsmVariables.GetFsmGameObject("SourceStorageGO").Value = sourceStorage ? sourceStorage.gameObject : null;
		fsm.FsmVariables.GetFsmGameObject("ItemGO").Value = item.gameObject;
		fsm.FsmVariables.GetFsmInt("Count").Value = count;
		fsm.SendEvent("Pick");
	}

	public void Put(int count)
	{
		fsm.FsmVariables.GetFsmInt("Count").Value = count;
		fsm.SendEvent("Put");
	}

	public void Put(int count, Storage storage)
	{
		fsm.FsmVariables.GetFsmGameObject("StackGO").Value = null;
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = storage ? storage.gameObject : null;
		Put(count);
	}

	public void Put(int count, Item stack)
	{
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = null;
		fsm.FsmVariables.GetFsmGameObject("StackGO").Value = stack ? stack.gameObject : null;
		Put(count);
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

	public void Store(Item item, Storage sourceStorage, Storage storage, int count)
	{
		fsm.FsmVariables.GetFsmGameObject("SourceStorageGO").Value = sourceStorage ? sourceStorage.gameObject : null;
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = storage.gameObject;
		fsm.FsmVariables.GetFsmGameObject("ItemGO").Value = item.gameObject;
		fsm.FsmVariables.GetFsmInt("Count").Value = count;
		fsm.SendEvent("Store");
	}

	public void Gather(Source source, Storage storage = null, int count = 0)
	{
		fsm.FsmVariables.GetFsmGameObject("SourceGO").Value = source.gameObject;
		fsm.FsmVariables.GetFsmGameObject("StorageGO").Value = storage ? storage.gameObject : null;
		fsm.FsmVariables.GetFsmInt("Count").Value = count;
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

	public void Follow(Target target)
	{
		fsm.FsmVariables.GetFsmObject("Target").Value = target;
		fsm.SendEvent("Follow");
	}

	public void Attack(Target target)
	{
		fsm.FsmVariables.GetFsmObject("Target").Value = target;
		fsm.FsmVariables.GetFsmGameObject("TargetGO").Value = target.gameObject;
		fsm.SendEvent("Attack");
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
