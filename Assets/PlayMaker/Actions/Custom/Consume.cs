using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]
	public class Consume : FsmStateAction
	{
		const float animationTimer = 0.2f;

		float timer;
		Citizen citizen;

		public override void OnEnter()
		{
			citizen = Owner.GetComponent<Citizen>();
			if (!citizen.pickedItem)
				Debug.LogError("No picked item to consume.", citizen);
			else if (citizen.pickedItem.type.consumableValue == 0)
				Debug.LogError(citizen.pickedItem + " is not consumable.", citizen);
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
				//citizen.stats.Set(citizen.pickedItem.type.consumableStat, citizen.pickedItem.type.consumableValue, true);
				Object.Destroy(citizen.pickedItem.gameObject);
				citizen.pickedItem = null;
				Finish();
			}
		}


	}

}
