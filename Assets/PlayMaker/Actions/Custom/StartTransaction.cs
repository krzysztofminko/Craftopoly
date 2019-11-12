using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class StartTransaction : FsmStateAction
	{

		[CheckForComponent(typeof(Item))]
		public FsmGameObject _storage;

		Storage storage;
		Citizen citizen;

		public override void OnEnter()
		{
			citizen = Owner.GetComponent<Citizen>();
			storage = _storage.Value ? _storage.Value.GetComponent<Storage>() : null;
			if (!storage || !storage.target.shopStructure)
				Finish();
		}

		public override void OnExit()
		{
			_storage.Value = null;
		}

		public override void OnUpdate()
		{
			if (citizen.GoTo(storage.transform))
			{
				if (!storage.target.ReservedBy)
				{
					storage.target.shopStructure.StartTransaction(citizen);
				}
				else if (storage.target.ReservedBy != citizen)
				{
					Fsm.Event("FAILED");
				}
				else if (storage.target.shopStructure.shopkeeperAvailable)
				{
					Finish();
				}
			}
		}
	}

}
