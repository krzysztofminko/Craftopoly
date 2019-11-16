using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace CitizenTasks
{
	[TaskCategory("Citizen")]
	public class Build : Action
	{
		public SharedGameObject _structure;
		public SharedGameObject returnStructure;

		private Citizen citizen;
		private Structure structure;
		private Blueprint blueprint;
		private float timer;

		public override void OnStart()
		{
			if (!citizen)
				citizen = gameObject.GetComponent<Citizen>();
			structure = _structure.Value.GetComponent<Structure>();
			blueprint = structure.next.blueprint;
			timer = 0;
		}

		public override TaskStatus OnUpdate()
		{
			if (!_structure.Value)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				return TaskStatus.Failure;
			}
			else
			{
				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > blueprint.duration / Mathf.Max(0.1f, citizen.skills.Get(Skills.Name.Building)))
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					for (int i = 0; i < blueprint.requiredItems.Count; i++)
						structure.storage.DestroyItemType(blueprint.requiredItems[i].type, blueprint.requiredItems[i].count);

					Structure builded = Object.Instantiate(structure.next, structure.transform.position, structure.transform.rotation);
					builded.name = structure.next.name;
					Object.Destroy(structure.gameObject);
					returnStructure.Value = builded.gameObject;

					return TaskStatus.Success;
				}
			}
			return TaskStatus.Running;
		}
	}
}