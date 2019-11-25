using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNodes.Conditionals
{
	[TaskCategory("Structure")]
	public class GetNearestStorageStructure : Conditional
	{
		[SharedRequired]
		public SharedGameObject originStructure;
		[SharedRequired]
		public SharedGameObject outStorageStructure;

		public override TaskStatus OnUpdate()
		{
			outStorageStructure.Value = null;

			if (SearchFor.NearestStorageStructure(originStructure.Value.GetComponent<Structure>().plot, originStructure.Value.transform.position, out StorageStructure targetStorage))
				outStorageStructure.Value = targetStorage.gameObject;

			return outStorageStructure.Value ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}