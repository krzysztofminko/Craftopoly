using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Follow : FsmStateAction
	{
		[CheckForComponent(typeof(Target))]
		public FsmObject _target;
		public FsmFloat proximity;

		Citizen citizen;
		Target target;

		public override void OnEnter()
		{
			if (!citizen)
				citizen = Owner.GetComponent<Citizen>();
			if (_target.Value)
			{
				target = (Target)_target.Value;
				if(proximity.IsNone)
					proximity.Value = citizen.target.proximity + target.proximity;
			}
			else
			{
				target = null;
			}
		}

		public override void OnExit()
		{
			_target.Value = null;
		}

		public override void OnUpdate()
		{
			if (!_target.Value)
			{
				Fsm.Event("FAILED");
			}
			else
			{
				float velocity = target.citizen.characterController.velocity.sqrMagnitude;
				if (Distance.Manhattan2D(citizen.transform.position, target.transform.position) > proximity.Value)
				{
					citizen.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(target.transform.position - citizen.transform.position, Vector3.up), Vector3.up);
					citizen.transform.position = Vector3.MoveTowards(citizen.transform.position, target.transform.position, citizen.walkSpeed * Time.deltaTime);
					citizen.animator.SetBool("Walk", true);
				}
				else if (velocity > 0)
				{
					citizen.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(target.transform.position - citizen.transform.position, Vector3.up), Vector3.up);
					citizen.transform.position = Vector3.MoveTowards(citizen.transform.position, target.transform.position, Mathf.Min(citizen.walkSpeed, velocity) * Time.deltaTime);
					citizen.animator.SetBool("Walk", true);
				}
				else
				{
					citizen.animator.SetBool("Walk", false);
				}
			}
		}
	}

}
