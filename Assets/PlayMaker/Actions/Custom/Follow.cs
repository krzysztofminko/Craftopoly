using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Follow : FsmStateAction
	{
		[CheckForComponent(typeof(Citizen))]
		public FsmObject _targetCitizen;
		public FsmFloat proximity;

		Citizen citizen;
		Citizen targetCitizen;

		public override void OnEnter()
		{
			if (!citizen)
				citizen = Owner.GetComponent<Citizen>();
			if (_targetCitizen.Value)
			{
				targetCitizen = (Citizen)_targetCitizen.Value;
				if(proximity.IsNone)
					proximity.Value = 2;
			}
			else
			{
				targetCitizen = null;
			}
		}

		public override void OnExit()
		{
			_targetCitizen.Value = null;
		}

		public override void OnUpdate()
		{
			if (!_targetCitizen.Value)
			{
				Fsm.Event("FAILED");
			}
			else
			{
				float velocity = targetCitizen.characterController.velocity.sqrMagnitude;
				if (Distance.Manhattan2D(citizen.transform.position, targetCitizen.transform.position) > proximity.Value)
				{
					citizen.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetCitizen.transform.position - citizen.transform.position, Vector3.up), Vector3.up);
					citizen.transform.position = Vector3.MoveTowards(citizen.transform.position, targetCitizen.transform.position, citizen.walkSpeed * Time.deltaTime);
					citizen.animator.SetBool("Walk", true);
				}
				else if (velocity > 0)
				{
					citizen.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetCitizen.transform.position - citizen.transform.position, Vector3.up), Vector3.up);
					citizen.transform.position = Vector3.MoveTowards(citizen.transform.position, targetCitizen.transform.position, Mathf.Min(citizen.walkSpeed, velocity) * Time.deltaTime);
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
