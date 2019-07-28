using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Interrupt : FsmStateAction
	{
		Citizen citizen;

		public override void OnEnter()
		{
			citizen = Owner.GetComponent<Citizen>();
			citizen.animator.SetBool("Walk", false);
			citizen.animator.SetFloat("UseAnimationId", 0);
			if (citizen.pickedItem)
			{
				citizen.pickedItem.SetParent(null);
				citizen.pickedItem = null;
			}
			Finish();
		}
	}

}


			