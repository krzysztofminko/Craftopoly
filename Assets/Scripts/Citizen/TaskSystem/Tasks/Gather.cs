using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	[System.Serializable]
	public class Gather : CitizenTask
	{
		private const float animationTimer = 0.2f;

		public Source source;

		private float timer;

		public Gather(Source source)
		{
			target = source.transform;

			this.source = source;
		}

		public override string ToString()
		{
			return $"{base.ToString()}({(source ? source.name : "null")})";
		}

		protected override void Start()
		{
			base.Start();

			timer = 0;
		}

		protected override TaskState Execute()
		{
			if (!source)
			{
				receiver.Log(this, "source == null");
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskState.Failure;
			}
			/*
			if (citizen.skills.Get(source.itemType.requiredSkill.name) < source.itemType.requiredSkill.value)
			{
				if (citizen == Player.instance)
					Utilities.UI.Notifications.instance.Add(source.itemType.requiredSkill.name + " " + source.itemType.requiredSkill.value + " required.");
				return TaskState.Failure;
			}
			*/
			if (citizen.GoTo(source.transform))
			{
				source.ReservedBy = citizen;

				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > animationTimer)
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					Item item = source.Gather(20 * Mathf.Max(0.1f, citizen.skills.Get(source.itemType.requiredSkill.name)));
					if (item)
					{
						source.ReservedBy = null;
						return TaskState.Success;
					}

					timer = 0;
				}
			}

			return TaskState.Running;
		}
	}
}