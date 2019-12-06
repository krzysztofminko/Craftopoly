using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public class TaskReceiver : SerializedMonoBehaviour
	{
		[SerializeField]
		private bool log;
		[ShowIf("log")]
		[SerializeField]
		private bool toConsole = true;

		public bool HasNoTasks { get => tasks.Count == 0; }

		[SerializeField]
		private Stack<Task> tasks = new Stack<Task>();

		[ShowIf("log")]
		[HideIf("toConsole")]
		[ReadOnly]
		[SerializeField]
		private List<string> logBook = new List<string>();

		public Task Receive(Task task)
		{
			Log(task, "Task received.");
			tasks.Push(task);
			task.receiver = this;
			return task;
		}
		
		private void Update()
		{
			if (tasks.Count > 0)
				if (tasks.Peek().Update())
					tasks.Pop();
		}

		public void Log(Task task, string logText)
		{
			if (log)
			{
				if (toConsole)
					Debug.Log($"{Time.frameCount} {name}: {task.ToString()} {logText}", this);
				else
					logBook.Add($"{Time.frameCount} {task.ToString()} {logText}");
			}
		}
	}
}