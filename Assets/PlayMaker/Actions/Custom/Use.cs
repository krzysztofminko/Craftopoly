using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Use : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Target))]
		[RequiredField]
		public FsmGameObject _target;
		public FsmGameObject spawnedResource;

		float timer;
		Citizen citizen;
		Target target;

		public override void OnExit()
		{
			timer = 0;
			target.ReservedBy = null;
			_target.Value = null;
		}

		public override void OnEnter()
		{
			if(!citizen)
				citizen = Owner.GetComponent<Citizen>();
			target = _target.Value? _target.Value.GetComponent<Target>() : null;
			target.ReservedBy = citizen;
		}

		public override void OnUpdate()
		{
			if (!_target.Value)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				Fsm.Event("FAILED");
			}
			else
			{
				citizen.animator.SetFloat("UseAnimationId", 1);
				

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);
					//...

					Finish();
				}
			}
		}


	}

}
