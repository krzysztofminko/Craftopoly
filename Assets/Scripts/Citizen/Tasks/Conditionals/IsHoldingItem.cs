using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks.Conditionals
{
	[TaskCategory("Citizen")]
	public class IsHoldingItem : Conditional
	{
		public SharedGameObject outItem;

		private Citizen citizen;

		public override TaskStatus OnUpdate()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();

			outItem.Value = citizen.pickedItem? citizen.pickedItem.gameObject : null;

			return citizen.pickedItem ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}