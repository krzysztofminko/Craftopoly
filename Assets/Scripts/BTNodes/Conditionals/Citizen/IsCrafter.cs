using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Conditionals
{
	[TaskCategory("Citizen")]
	public class IsCrafter : CitizenConditional
	{
		public SharedGameObject outCraftStructure;

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();

			CraftStructure cs = citizen.workplace ? citizen.workplace.GetComponent<CraftStructure>() : null;
			outCraftStructure.Value = cs ? cs.gameObject : null;

			return cs ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}