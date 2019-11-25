using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BTVariables;
using System.Linq;

namespace BTNodes.Conditionals
{
	[TaskCategory("Structure")]
	public class GetItemInStorages : Conditional
	{
		[SharedRequired]
		public SharedGameObject originGameObject;
		public SharedItemType itemType;

		public SharedGameObject outItem;
		public SharedGameObject outStorage;

		public override TaskStatus OnUpdate()
		{
			if (SearchFor.ItemInStorageStructures(itemType.Value, originGameObject.Value.transform.position, out Item item, out Storage storage, originGameObject.Value.GetComponent<StorageStructure>()))
			{
				outItem.Value = item.gameObject;
				outStorage.Value = storage.gameObject;
				return TaskStatus.Success;
			}
			else
			{
				outItem.Value = null;
				outStorage.Value = null;
				return TaskStatus.Failure;
			}

		}
	}
}