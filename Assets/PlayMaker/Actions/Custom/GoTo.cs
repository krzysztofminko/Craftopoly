using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class GoTo : FsmStateAction
	{
		[CheckForComponent(typeof(Target))]
		public FsmGameObject targetGameObject;
		[HideIf("targetGameObjectNotNull")]
		public FsmVector3 position;
		[HideIf("targetGameObjectNotNull")]
		public FsmFloat proximity;
		[HideIf("targetGameObjectNull")]
		public FsmBool reserveTarget;

		Citizen citizen;
		Target target;


		public bool targetGameObjectNotNull()
		{
			return !targetGameObject.IsNone;
		}

		public bool targetGameObjectNull()
		{
			return targetGameObject.IsNone;
		}

		public override void OnExit()
		{
			if (reserveTarget.Value)
				target.ReservedBy = null;
			targetGameObject.Value = null;
		}

		public override void OnEnter()
		{
			if(!citizen)
				citizen = Owner.GetComponent<Citizen>();
			if (targetGameObject.Value)
			{
				target = targetGameObject.Value.GetComponent<Target>();
				position.Value = target.transform.position;
				proximity.Value = 1;
			}
			else
			{
				//Target = null, don't go anywhere
				//target = null;
				//Finish();
			}

			if (reserveTarget.Value)
				target.ReservedBy = citizen;
		}		

		public override void OnUpdate()
		{
			if (citizen.GoTo(position.Value, proximity.Value))
				Finish();

			/*
			if (Distance.Manhattan2D(citizen.transform.position, position.Value) > proximity.Value)
			{
				citizen.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(position.Value - citizen.transform.position, Vector3.up), Vector3.up);
				citizen.transform.position = Vector3.MoveTowards(citizen.transform.position, position.Value, citizen.walkSpeed * Time.deltaTime);
				citizen.animator.SetBool("Walk", true);
			}
			else
			{
				citizen.animator.SetBool("Walk", false);
				Finish();
			}
			*/
		}


	}

}
