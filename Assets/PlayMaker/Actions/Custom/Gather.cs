using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Gather : FsmStateAction
	{
		const float animationTimer = 0.2f;

		[CheckForComponent(typeof(Target))]
		[RequiredField]
		public FsmGameObject _source;
		public FsmGameObject returnItem;

		float timer;
		Citizen citizen;
		Source source;

		public override void OnEnter()
		{
			if (!citizen)
				citizen = Owner.GetComponent<Citizen>();
			source = _source.Value? _source.Value.GetComponent<Source>() : null;
		}

		public override void OnExit()
		{
			timer = 0;
			_source.Value = null;
		}

		public override void OnUpdate()
		{
			if (!_source.Value)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				Fsm.Event("FAILED");
			}
			if (citizen.skills.Get(source.itemType.requiredSkill.name) < source.itemType.requiredSkill.value)
			{
				if (citizen == Player.instance)
					Utilities.UI.Notifications.instance.Add(source.itemType.requiredSkill.name + " " + source.itemType.requiredSkill.value + " required.");
				Fsm.Event("FAILED");
			}
			else if(citizen.GoTo(source.target))
			{
				source.target.ReservedBy = citizen;

				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					Item item = source.Gather(20 * Mathf.Max(0.1f, citizen.skills.Get(source.itemType.requiredSkill.name)));
					if (item)
					{
						returnItem.Value = item.gameObject;
						source.target.ReservedBy = null;
						Finish();
					}
				}
			}
		}


	}

}
