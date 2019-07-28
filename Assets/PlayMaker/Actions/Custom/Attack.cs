using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Attack : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Target))]
		public FsmObject _target;
		public FsmFloat proximity;

		float timer;
		Citizen citizen;
		Target target;

		public override void OnExit()
		{
			timer = 0;
			_target.Value = null;
		}

		public override void OnEnter()
		{
			if (!citizen)
				citizen = Owner.GetComponent<Citizen>();
			if (_target.Value)
			{
				target = (Target)_target.Value;
				if (proximity.IsNone)
					proximity.Value = citizen.target.proximity + target.proximity;
			}
			else
			{
				target = null;
			}

		}

		public override void OnUpdate()
		{
			if (!_target.Value)
			{
				Fsm.Event("FAILED");
			}
			else
			{
				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					if (!target.Damage(20 * Mathf.Max(0.1f, citizen.skills.Get(Skills.Name.Fight))))
						Fsm.Event("Survived");
					else
						Finish();
				}
			}
		}
	}

}
