using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class AttachTool : FsmStateAction
	{
		const float animationTimer = 0.2f;

		float timer;
		Citizen citizen;

		public override void OnEnter()
		{
			timer = 0;
			citizen = Owner.GetComponent<Citizen>();

			if (citizen.pickedItem.type.attachParent == ItemType.AttachParent.None)
				Debug.LogError("Picked item (" + citizen.pickedItem.name + ") is not a tool.", citizen);
		}

		public override void OnUpdate()
		{
			citizen.animator.SetFloat("UseAnimationId", 1);

			timer += Time.deltaTime;
			if (timer > animationTimer)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);

				Item tmpTool = null;
				if (citizen.attachedTool)
					tmpTool = citizen.attachedTool;

				citizen.attachedTool = citizen.pickedItem;
				citizen.pickedItem = tmpTool;
				
				Finish();
			}
		}
	}

}
