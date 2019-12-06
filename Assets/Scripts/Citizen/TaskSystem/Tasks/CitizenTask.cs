using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public abstract class CitizenTask : Task
	{
		protected Citizen citizen;

		protected override void Start()
		{
			citizen = receiver.GetComponent<Citizen>();
		}
	}
}