using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Conditionals
{
	[TaskCategory("Citizen")]
	public class GetCraftData : CitizenConditional
	{
		public SharedGameObject outStorage;
		public SharedGameObject outCraftedItem;

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();

			return TaskStatus.Success;
		}
	}
}