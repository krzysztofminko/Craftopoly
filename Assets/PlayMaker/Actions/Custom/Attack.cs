using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Attack : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Health))]
		public FsmObject _health;

		float timer;
		Citizen citizen;
		Health health;

		public override void OnExit()
		{
			timer = 0;
			_health.Value = null;
		}

		public override void OnEnter()
		{
			if (!citizen)
				citizen = Owner.GetComponent<Citizen>();
			health = _health.Value? (Health)_health.Value : null;
		}

		public override void OnUpdate()
		{
			if (!_health.Value)
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

					if (!health.Damage(20 * Mathf.Max(0.1f, citizen.skills.Get(Skills.Name.Fight))))
						Fsm.Event("Survived");
					else
						Finish();
				}
			}
		}
	}

}
