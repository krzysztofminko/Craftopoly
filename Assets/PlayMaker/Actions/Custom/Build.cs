using UnityEngine;

//#TODO: Instantiate Structure before Build starts, and mark it "finished" after it.
//#TODO: Partial Build
namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class Build : FsmStateAction
	{
		[CheckForComponent(typeof(Structure))]
		[RequiredField]
		public FsmGameObject _structure;
		public FsmGameObject returnStructure;

		float timer;
		Citizen citizen;
		Structure structure;
		Blueprint blueprint;

		public override void OnExit()
		{
			timer = 0;
			structure.ReservedBy = null;
			_structure.Value = null;
		}

		public override void OnEnter()
		{
			if (!citizen)
				citizen = Owner.GetComponent<Citizen>();
			structure = _structure.Value.GetComponent<NewStructure>();
			blueprint = structure.next.blueprint;
			structure.ReservedBy = citizen;
		}

		public override void OnUpdate()
		{
			if (!_structure.Value)
			{
				citizen.animator.SetFloat("UseAnimationId", 0);
				Fsm.Event("FAILED");
			}
			else
			{
				citizen.animator.SetFloat("UseAnimationId", 1);

				timer += Time.deltaTime;
				if (timer > blueprint.duration / Mathf.Max(0.1f, citizen.skills.Get(Skills.Name.Building)))
				{
					citizen.animator.SetFloat("UseAnimationId", 0);

					for (int i = 0; i < blueprint.requiredItems.Count; i++)
						structure.storage.DespawnItemType(blueprint.requiredItems[i].type, blueprint.requiredItems[i].count);

					Structure builded = Object.Instantiate(structure.next, structure.transform.position, structure.transform.rotation);
					builded.name = structure.next.name;
					Object.Destroy(structure.gameObject);
					returnStructure.Value = builded.gameObject;

					Finish();
				}
			}
		}


	}

}
