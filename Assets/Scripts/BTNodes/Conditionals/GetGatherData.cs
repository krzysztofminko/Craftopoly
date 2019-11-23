using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

namespace BTNodes.Conditionals
{
	[TaskCategory("Citizen")]
	public class GetGatherData : CitizenConditional
	{
		public SharedGameObject outStorage;
		public SharedGameObject outSource;
		public SharedGameObject outItem;

		/* 
		 * is gatherer
		 * search storage
		 * search item on the ground
		 * search source
		 * store item
		 * gather source
		 */

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();

			outItem.Value = null;
			outStorage.Value = null;
			outSource.Value = null;

			GatherStructure gatherStructure = citizen.workplace.GetComponent<GatherStructure>();
			if (gatherStructure)
			{
				if (SearchFor.NearestStorageStructure(gatherStructure.plot, transform.position, out StorageStructure storage))
					outStorage.Value = storage.gameObject;
				else
					//TODO: "No storage" notification
					return TaskStatus.Failure;

				if (SearchFor.ItemOnTheGround(gatherStructure.itemType, gatherStructure.plot, gatherStructure.transform.position, out GameObject item))
				{
					outItem.Value = item;
				}
				else
				{
					Source source = gatherStructure.sources.FirstOrDefault(s => s && s.gameObject.activeSelf);
						//Source.list.FindAll(s => s.itemType == gatherStructure.itemType && !s.ReservedBy && s.Health.HP > 0 && Distance.Manhattan2D(gatherStructure.transform.position, s.transform.position) < gatherStructure.rangeOfSearch).OrderBy(s => Distance.Manhattan2D(gatherStructure.transform.position, s.transform.position)).FirstOrDefault();
					if (source)
						outSource.Value = source.gameObject;
				}

				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}