using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace CitizenTasks.Search
{
	[TaskCategory("Citizen/SearchFor")]
	public class ItemOnTheGround : Conditional
	{
		public SharedObject itemType;
		public SharedGameObject plot;
		private SharedVector3 position;

		public SharedGameObject returnedItem;
		private GameObject ri;

		public override TaskStatus OnUpdate()
		{
			if (SearchFor.ItemOnTheGround((ItemType)itemType.Value, plot.Value ? plot.Value.GetComponent<Plot>() : null, position.Value, out ri))
			{
				returnedItem.Value = ri;
				return TaskStatus.Success;
			}
			else
			{
				return TaskStatus.Failure;
			}
		}
	}
}