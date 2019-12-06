using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
	public class Store : CitizenTask
	{
		private Item item;
		private Storage sourceStorage;
		private Storage targetStorage;

		private Queue<Task> sequence = new Queue<Task>();
		private Task activeTask;

		public Store(Item item, Storage targetStorage, Storage sourceStorage = null)
		{
			target = item.transform;

			this.item = item;
			this.targetStorage = targetStorage;
			this.sourceStorage = sourceStorage;
		}

		protected override void Start()
		{
			base.Start();

			sequence.Enqueue(new Pick(item, sourceStorage));
			sequence.Enqueue(new Put(targetStorage));
		}

		public override string ToString()
		{
			return $"{base.ToString()}({(item ? item.name : "null")}, from {(sourceStorage ? sourceStorage.name : "ground")}, to {(targetStorage ? targetStorage.name : "null")})";
		}

		protected override TaskState Execute()
		{
			if(activeTask != null && activeTask.state == TaskState.Failure)
				return TaskState.Failure;

			if (sequence.Count == 0)
				return TaskState.Success;

			activeTask = sequence.Dequeue();
			receiver.Receive(activeTask);

			return TaskState.Running;
		}
	}
}