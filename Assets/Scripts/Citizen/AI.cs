using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tasks;

public class AI : MonoBehaviour
{
	private Citizen citizen;
	private Tasks.TaskReceiver taskReceiver;

	private void Awake()
	{
		citizen = GetComponent<Citizen>();
		taskReceiver = GetComponent<TaskReceiver>();
	}

	private void Update()
	{
		if (taskReceiver.HasNoTasks)
		{
			Task task = null;

			for (int i = 0; task == null && i < TaskProvider.list.Count; i++)
				task = TaskProvider.list[i].tasks.Find(t => !t.receiver);

			if (task != null)
				taskReceiver.Receive(task);
		}
	}
}
