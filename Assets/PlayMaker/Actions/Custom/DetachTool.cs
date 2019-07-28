using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class DetachTool : FsmStateAction
	{
		const float animationTimer = 0.2f;

		float timer;
		Citizen citizen;

		public override void OnEnter()
		{
			citizen = Owner.GetComponent<Citizen>();
		}

		public override void OnExit()
		{
			timer = 0;
		}

		public override void OnUpdate()
		{
			citizen.animator.SetFloat("UseAnimationId", 1);

			timer += Time.deltaTime;
			if (timer > animationTimer)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);

				citizen.pickedItem = citizen.attachedTool;
				citizen.attachedTool = null;

				Finish();
			}
		}
	}

}
