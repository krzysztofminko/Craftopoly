using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Conditionals
{
	[TaskCategory("Citizen")]
	public class IsGatherer : CitizenConditional
	{
		public SharedGameObject outGatherStructure;

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();

			GatherStructure gs = citizen.workplace ? citizen.workplace.GetComponent<GatherStructure>() : null;
			outGatherStructure.Value = gs ? gs.gameObject : null;

			return gs ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}