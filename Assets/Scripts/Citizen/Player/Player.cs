﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.UI;

public class Player : Citizen
{
	public static Player instance;

	[Header("Runtime")]
	[Header("Player")]
	public bool controlsEnabled = true;
	public Target focused;

	public override void Awake()
	{
		base.Awake();
		instance = this;
		list.Remove(this);
	}

	void Update()
	{
		if (controlsEnabled && fsm.ActiveStateName == "Idle")
		{
			#region Focusing

			List<Collider> colliders = new List<Collider>();
			colliders = Physics.OverlapSphere(transform.position + transform.forward, 4).OrderBy(c => Distance.Manhattan2D(transform.position + transform.forward, c.transform.position)).ToList();
			colliders.Remove(this.GetComponent<Collider>());
			colliders.RemoveAll(c => !c.GetComponent<Target>());
			if (colliders.Count > 0)
			{
				Focus(colliders[0].GetComponent<Target>());
			}
			else if (focused)
			{
				Unfocus();
			}

			#endregion

			#region  Controls

			if (Input.GetButtonDown("ManageWorkers"))
			{
				UI.WorkplaceCanvas.instance.Show();
			}
			
			if (pickedItem && pickedItem.type.consumableValue > 0)
			{
				if(InputHints.GetButtonDown("Consume"))
					fsm.Consume();
			}

			if (pickedItem && pickedItem.type.attachParent != ItemType.AttachParent.None)
			{
				if (InputHints.GetButtonDown("AttachTool", "Attach " + pickedItem.name))
					fsm.AttachTool();
			}
			else if (!pickedItem && attachedTool)
			{
				if (Input.GetButtonDown("AttachTool"))
					fsm.DetachTool();
			}
			
			if (focused)
			{
				if (focused.gatherStructure)
				{
					if (InputHints.GetButtonDown("Manage", "Manage"))
						UI.GatherCanvas.instance.Show(focused.gatherStructure);
				}
				else if(focused.craftStructure)
				{
					if (InputHints.GetButtonDown("Manage", "Manage"))
						UI.CraftingCanvas.instance.Show(focused.craftStructure);
				}
				else if (focused.shopStructure)
				{
					if (InputHints.GetButtonDown("Manage", "Manage"))
						UI.StorageCanvas.instance.Show(focused.shopStructure.storage, focused.shopStructure);
				}

				if (pickedItem)
				{
					if (focused.shopStructure && focused.shopStructure.plot)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Sell for " + pickedItem.type.value * pickedItem.count))
							if (focused.shopStructure.plot.Money < pickedItem.type.value * pickedItem.count)
							{
								Notifications.instance.Add("Not enough money in shop.");
							}
							else if (!focused.shopStructure.shopkeeperAvailable)
							{
								Notifications.instance.Add("Wait for shopkeeper.");
							}
							else
							{
								focused.shopStructure.plot.Pay(this, pickedItem.type.value * pickedItem.count);
								fsm.Put(pickedItem.count, focused.shopStructure.storage);
							}
					}
					else if (focused.storage)
					{
						bool refuel = pickedItem.type.fuelValue > 0 && focused.craftStructure && focused.craftStructure.fuelMax > 0;
						if (InputHints.GetButtonDown("PrimaryAction", refuel? "Refuel" : "Put"))
							fsm.Put(pickedItem.count, focused.storage);
					}
					else if (focused.item)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Stack"))
							fsm.Put(pickedItem.count, focused.item);
					}
				}
				else
				{
					if (focused.source)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Gather"))
							fsm.Gather(focused.source);
					}
					else if (focused.item)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Pick " + focused.item.name))
							fsm.Pick(null, focused.item, focused.item.count);
					}
					else if (focused.newStructure)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Build"))
						{
							List<ItemCount> missing = focused.newStructure.next.GetComponent<Blueprint>().MissingResources(focused.newStructure.storage);
							if (missing.Count > 0)
							{
								string itemsListString = "";
								foreach (var m in missing)
									itemsListString += ("\n " + Localization.Translate(m.type.name) + " x" + m.count);
								Utilities.UI.Notifications.instance.Add(Localization.Translate("MISSING_ITEMS") + ": " + itemsListString);
							}
							else
							{
								fsm.Build(focused.newStructure);
							}
						}
					}
					else if (focused.craftStructure)
					{
						if (focused.craftStructure.craftedItem)
						{
							if (InputHints.GetButtonDown("PrimaryAction", "Pick " + focused.craftStructure.craftedItem.name))
							{
								fsm.Pick(focused.storage, focused.craftStructure.craftedItem, focused.craftStructure.craftedItem.count);
								focused.craftStructure.craftedItem = null;
							}
						}
						else if(focused.craftStructure.currentItemType && (!focused.craftStructure.worker || focused.craftStructure.worker == this))
						{
							if (InputHints.GetButtonDown("PrimaryAction", "Craft " + focused.craftStructure.currentItemType.name))
							{
								List<ItemCount> missing = focused.craftStructure.currentItemType.blueprint.MissingResources(focused.craftStructure.storage);
								if (missing.Count > 0)
								{
									string itemsListString = "";
									foreach (var m in missing)
										itemsListString += ("\n " + Localization.Translate(m.type.name) + " x" + m.count);
									Utilities.UI.Notifications.instance.Add(Localization.Translate("MISSING_ITEMS") + ": " + itemsListString);
								}
								else
								{
									fsm.Craft(focused.craftStructure.currentItemType, focused.craftStructure);
									focused.craftStructure.currentItemType = null;
								}
							}
						}//Craft currentItemType
					}
					else if (focused.gatherStructure)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Manage"))
							UI.GatherCanvas.instance.Show(focused.gatherStructure);
					}
					else if (focused.shopStructure)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Buy"))
							if (focused.shopStructure.shopkeeperAvailable)
								UI.StorageCanvas.instance.Show(focused.shopStructure.storage, focused.shopStructure);
							else
								Notifications.instance.Add("Wait for shopkeeper.");

					}
					
					if (focused.storage)
					{
						if (InputHints.GetButtonDown("SecondaryAction", "Storage"))
							UI.StorageCanvas.instance.Show(focused.storage);
					}
				}

			}

			if (pickedItem)
			{
				if (InputHints.GetButtonDown("SecondaryAction", "Put down"))
					fsm.Put(pickedItem.count);
			}
			#endregion
		}
	}


	public void Focus(Target t)
	{
		if (focused && focused != t)
			Unfocus();
		t.Focus();
		focused = t;
		UI.FocusCanvas.instance.Show(t);
		foreach (var f in focused.GetComponents<IOnPlayerFocus>())
			f.OnPlayerFocus();
	}

	public void Unfocus()
	{
		foreach (var f in focused.GetComponents<IOnPlayerUnfocus>())
			f.OnPlayerUnfocus();
		if (UI.FocusCanvas.instance)
			UI.FocusCanvas.instance.Hide();
		focused.Unfocus();
		focused = null;
	}
}
