using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public class TaskProvider : SerializedMonoBehaviour
	{
		public static List<TaskProvider> list = new List<TaskProvider>();

		public List<Task> tasks = new List<Task>();

		private void Awake()
		{
			list.Add(this);
		}

		private void OnDestroy()
		{
			list.Remove(this);
		}
	}
}