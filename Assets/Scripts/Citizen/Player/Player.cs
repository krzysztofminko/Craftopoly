using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.UI;

public class Player : Citizen
{
	public static Player instance;

	[Header("Runtime")]
	[Header("Player")]
	public bool controlsEnabled = true;
	public FocusTarget focusedOn;

	private TaskReceiver taskReceiver;

	protected override void Awake()
	{
		base.Awake();
		instance = this;
		list.Remove(this);
		taskReceiver = GetComponent<TaskReceiver>();
	}

	void Update()
	{
		if (controlsEnabled && fsm.ActiveStateName == "Idle")
		{
			#region Focusing

			List<Collider> colliders = new List<Collider>();
			colliders = Physics.OverlapSphere(transform.position + transform.forward, 1).OrderBy(c => Distance.Manhattan2D(transform.position + transform.forward, c.transform.position)).ToList();
			colliders.Remove(this.GetComponent<Collider>());
			colliders.RemoveAll(c => !c.GetComponent<FocusTarget>());
			if (colliders.Count > 0)
			{
				Focus(colliders[0].GetComponent<FocusTarget>());
			}
			else if (focusedOn)
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
			
			if (focusedOn)
			{
				if (focusedOn.GetComponent<GatherStructure>())
				{
					if (InputHints.GetButtonDown("Manage", "Manage"))
						UI.GatherCanvas.instance.Show(focusedOn.GetComponent<GatherStructure>());
				}
				else if (focusedOn.GetComponent<CraftStructure>())
				{
					if (InputHints.GetButtonDown("Manage", "Manage"))
						UI.CraftingCanvas.instance.Show(focusedOn.GetComponent<CraftStructure>());
				}
				else if (focusedOn.GetComponent<ShopStructure>())
				{
					if (InputHints.GetButtonDown("Manage", "Manage"))
						UI.StorageCanvas.instance.Show(focusedOn.GetComponent<ShopStructure>().storage, focusedOn.GetComponent<ShopStructure>());
				}

				if (pickedItem)
				{
					if (focusedOn.GetComponent<ShopStructure>() && focusedOn.GetComponent<ShopStructure>().plot)
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Sell for " + pickedItem.type.value))
							if (focusedOn.GetComponent<ShopStructure>().plot.Money < pickedItem.type.value)
							{
								Notifications.instance.Add("Not enough money in shop.");
							}
							else
							{
								focusedOn.GetComponent<ShopStructure>().plot.Pay(this, pickedItem.type.value);
								fsm.Put(focusedOn.GetComponent<ShopStructure>().storage);
							}
					}
					else if (focusedOn.GetComponent<Storage>())
					{						
						if (focusedOn.GetComponent<CraftStructure>())
						{
							bool refuel = pickedItem.type.fuelValue > 0 && focusedOn.GetComponent<CraftStructure>().fuelMax > 0;
							if (InputHints.GetButtonDown("PrimaryAction", refuel ? "Refuel" : "Put"))
							{
								if (focusedOn.GetComponent<CraftStructure>().CurrentItemType)
								{
									List<ItemCount> missing = focusedOn.GetComponent<CraftStructure>().CurrentItemType.blueprint.MissingResources(focusedOn.GetComponent<CraftStructure>().storage);
									ItemCount mic = missing.Find(m => m.type == pickedItem.type);
									if (mic != null)
									{
										fsm.Put(focusedOn.GetComponent<CraftStructure>().storage);
									}
									else
									{
										string itemsListString = "";
										foreach (var m in missing)
											itemsListString += ("\n " + Localization.Translate(m.type.name) + " x" + m.count);
										Utilities.UI.Notifications.instance.Add(Localization.Translate("MISSING_ITEMS") + ": " + itemsListString);
									}
								}
							}
						}
						else if (InputHints.GetButtonDown("PrimaryAction", "Put"))
						{
							taskReceiver.Receive(new Put(focusedOn.GetComponent<Storage>()));
							//fsm.Put(focusedOn.GetComponent<Storage>());
						}
					}
				}
				else
				{
					if (focusedOn.GetComponent<Source>())
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Gather"))
							taskReceiver.Receive(new Gather(focusedOn.GetComponent<Source>()));
							//fsm.Gather(focusedOn.GetComponent<Source>());

					}
					else if (focusedOn.GetComponent<Item>())
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Pick " + focusedOn.GetComponent<Item>().name))
							taskReceiver.Receive(new Pick(focusedOn.GetComponent<Item>()));
							//fsm.Pick(focusedOn.GetComponent<Item>());
					}
					else if (focusedOn.GetComponent<NewStructure>())
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Build"))
						{
							List<ItemCount> missing = focusedOn.GetComponent<NewStructure>().next.GetComponent<Blueprint>().MissingResources(focusedOn.GetComponent<NewStructure>().storage);
							if (missing.Count > 0)
							{
								string itemsListString = "";
								foreach (var m in missing)
									itemsListString += ("\n " + Localization.Translate(m.type.name) + " x" + m.count);
								Utilities.UI.Notifications.instance.Add(Localization.Translate("MISSING_ITEMS") + ": " + itemsListString);
							}
							else
							{
								fsm.Build(focusedOn.GetComponent<NewStructure>());
							}
						}
					}
					else if (focusedOn.GetComponent<CraftStructure>())
					{
						if (focusedOn.GetComponent<CraftStructure>().CraftedItem)
						{
							if (InputHints.GetButtonDown("PrimaryAction", "Pick " + focusedOn.GetComponent<CraftStructure>().CraftedItem.name))
								fsm.Pick(focusedOn.GetComponent<Storage>(), focusedOn.GetComponent<CraftStructure>().CraftedItem);
						}
						else if(focusedOn.GetComponent<CraftStructure>().CurrentItemType && (!focusedOn.GetComponent<CraftStructure>().worker || focusedOn.GetComponent<CraftStructure>().worker == this))
						{
							if (InputHints.GetButtonDown("PrimaryAction", "Craft " + focusedOn.GetComponent<CraftStructure>().CurrentItemType.name))
							{
								List<ItemCount> missing = focusedOn.GetComponent<CraftStructure>().CurrentItemType.blueprint.MissingResources(focusedOn.GetComponent<CraftStructure>().storage);
								if (missing.Count > 0)
								{
									string itemsListString = "";
									foreach (var m in missing)
										itemsListString += ("\n " + Localization.Translate(m.type.name) + " x" + m.count);
									Utilities.UI.Notifications.instance.Add(Localization.Translate("MISSING_ITEMS") + ": " + itemsListString);
								}
								else
								{
									fsm.Craft(focusedOn.GetComponent<CraftStructure>().CurrentItemType, focusedOn.GetComponent<CraftStructure>());
									focusedOn.GetComponent<CraftStructure>().CurrentItemType = null;
								}
							}
						}//Craft currentItemType
					}
					else if (focusedOn.GetComponent<ShopStructure>())
					{
						if (InputHints.GetButtonDown("PrimaryAction", "Buy"))
							UI.StorageCanvas.instance.Show(focusedOn.GetComponent<ShopStructure>().storage, focusedOn.GetComponent<ShopStructure>());
					}

					if (focusedOn.GetComponent<Storage>())
					{
						if (InputHints.GetButtonDown("SecondaryAction", "Storage"))
							UI.StorageCanvas.instance.Show(focusedOn.GetComponent<Storage>());
					}
				}

			}

			if (pickedItem)
			{
				if (InputHints.GetButtonDown("SecondaryAction", "Put down"))
					fsm.Put();
			}
			#endregion
		}
	}


	public void Focus(FocusTarget f)
	{
		if (focusedOn && focusedOn != f)
			Unfocus();
		f.Focused = true;
		focusedOn = f;
		UI.FocusCanvas.instance.Show(f);
	}

	public void Unfocus()
	{
		if (UI.FocusCanvas.instance)
			UI.FocusCanvas.instance.Hide();
		focusedOn.Focused = false;
		focusedOn = null;
	}
}
