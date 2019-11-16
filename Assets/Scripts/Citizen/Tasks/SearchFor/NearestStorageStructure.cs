using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace CitizenTasks.Search
{
	[TaskCategory("Citizen/SearchFor")]
	public class NearestStorageStructure : Conditional
	{
		public SharedGameObject plot;
		private SharedVector3 position;

		public SharedGameObject returnedStorageStructure;
		private GameObject rss;

		public override TaskStatus OnUpdate()
		{
			if (SearchFor.NearestStorageStructure(plot.Value ? plot.Value.GetComponent<Plot>() : null, position.Value, out rss))
			{
				returnedStorageStructure.Value = rss;
				return TaskStatus.Success;
			}
			else
			{
				return TaskStatus.Failure;
			}
		}
	}
}