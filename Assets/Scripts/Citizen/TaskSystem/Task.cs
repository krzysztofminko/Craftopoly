using UnityEngine;

namespace Tasks
{
	public enum TaskState { Running, Success, Failure }

	[System.Serializable]
	public abstract class Task
	{
		public delegate void OnFinish(Task task);
		public event OnFinish onFinish;
		
		public TaskState state;
		public TaskReceiver receiver;
		public Transform target;

		private bool started;
		
		public override string ToString()
		{
			return $"{base.ToString()}[{GetHashCode()}]";
		}

		public bool Update()
		{
			if (!started)
			{
				receiver.Log(this, "Task starting.");
				Start();
				started = true;
			}

			state = Execute();

			if (state != TaskState.Running)
			{
				if (state == TaskState.Failure)
					receiver.Log(this, "Task failed.");

				if (state == TaskState.Success)
				{
					receiver.Log(this, "Task succeeded.");
				}

				onFinish?.Invoke(this);
				return true;
			}

			return false;
		}

		protected abstract void Start();
		protected abstract TaskState Execute();

	}
}