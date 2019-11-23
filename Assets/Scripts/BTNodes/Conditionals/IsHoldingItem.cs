using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Conditionals
{
	[TaskCategory("Citizen")]
	public class IsHoldingItem : CitizenConditional
	{
		public SharedGameObject outItem;
		
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();

			outItem.Value = citizen.pickedItem? citizen.pickedItem.gameObject : null;

			return citizen.pickedItem ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}