using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class StartTransaction : FsmStateAction
	{

		[CheckForComponent(typeof(ShopStructure))]
		public FsmGameObject _shopStructure;

		ShopStructure shopStructure;
		Storage storage;
		Citizen citizen;

		public override void OnEnter()
		{
			citizen = Owner.GetComponent<Citizen>();
			shopStructure = _shopStructure.Value ? _shopStructure.Value.GetComponent<ShopStructure>() : null;
			if (!shopStructure)
				Finish();
		}

		public override void OnExit()
		{
			_shopStructure.Value = null;
		}

		public override void OnUpdate()
		{
			if (citizen.GoTo(shopStructure.transform))
			{
				if (!shopStructure.ReservedBy)
				{
					shopStructure.StartTransaction(citizen);
				}
				else if (shopStructure.ReservedBy != citizen)
				{
					Fsm.Event("FAILED");
				}
				else if (shopStructure.shopkeeperAvailable)
				{
					Finish();
				}
			}
		}
	}

}
