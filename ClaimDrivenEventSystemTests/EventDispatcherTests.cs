namespace Acrux.EventSystems.ClaimDriven.Tests;

[TestClass]
public class EventDispatcherTests
{
	#region Test Single Generic
	class CommandSentData
	{
		public string[] Keywords { get; init; }
	}

	[TestMethod]
	public void TestSingleGeneric()
	{
		// Dispatcher creation
		ClaimEventDispatcher<CommandSentData> onCommandSent = new();

		// Listener 1
		ClaimEventListener<CommandSentData> printListener = new();
		printListener.Priority = 0;
		printListener.OnCorroborate = (CommandSentData data) =>
		{
			if (data.Keywords[0] == "print") return true;
			return false;
		};
		printListener.OnAccepted = (CommandSentData data) =>
		{
			Console.WriteLine(data.Keywords[1]);
		};
		onCommandSent.Add(printListener);

		// Listener 2
		ClaimEventListener<CommandSentData> sumListener = new();
		sumListener.Priority = 0;
		sumListener.OnCorroborate = (CommandSentData data) =>
		{
			if (data.Keywords[0] == "party") return true;
			return false;
		};
		sumListener.OnAccepted = (CommandSentData data) =>
		{
			Console.WriteLine("ITS TIME TO PARTY!!!");
		};
		onCommandSent.Add(sumListener);

		// Disptcher invokation
		onCommandSent.Invoke( new CommandSentData() { Keywords = ["print", "Hi"]} );
		onCommandSent.Invoke( new CommandSentData() { Keywords = ["party"] });
		onCommandSent.Invoke( new CommandSentData() { Keywords = ["invalid"] });
	}
	#endregion

	#region Test Multiple Generic
	public class Task
	{
		public string Kind { get; }
		private void MarkAsDone()
		{
			Console.WriteLine("Task marked as done!");
		}

		public static (Task task, TaskManager manager) New(string taskKind)
		{
			Task task = new(taskKind);
			TaskManager manager = new(task);

			return (task, manager);
		}

		private Task(string kind)
		{
			Kind = kind;
		}

		public class TaskManager
		{
			private Task Task { get; }
			public void MarkAsDone() => Task.MarkAsDone();
			public TaskManager(Task task) { Task = task; }
		}
	}

	[TestMethod]
	public void TestMultipleGeneric()
	{
		ClaimEventDispatcher<Task, (Task task, Task.TaskManager manager)> taskDeliveredEvent = new();

		// Student
		taskDeliveredEvent += new ClaimEventListener<Task, (Task, Task.TaskManager)>()
		{
			OnCorroborate = (Task task) => task.Kind == "Homework",
			OnAccepted = ((Task task, Task.TaskManager manager) args) =>
			{
				Console.WriteLine("Doing my homework...");
				args.manager.MarkAsDone();
			}
		};

		// Baker
		taskDeliveredEvent += new ClaimEventListener<Task, (Task task, Task.TaskManager manager)>()
		{
			OnCorroborate = (Task task) => task.Kind == "Bake",
			OnAccepted = ((Task task, Task.TaskManager manager) args) =>
			{
				Console.WriteLine("Baking...");
				args.manager.MarkAsDone();
			}
		};

		(Task task, Task.TaskManager manager) = Task.New("Bake");
		taskDeliveredEvent.Invoke(task, (task, manager));
	}
	#endregion

	#region
	class MultipartTask(string kind)
	{
		public string Kind { get; } = kind;
		bool IsPartOneDone { get; set; }
		bool IsPartTwoDone { get; set; }
		bool IsPartTreeDone { get; set; }


		public class MultipartTaskManager(MultipartTask task)
		{
			MultipartTask Task { get; } = task;
			public void FinishPartOne() 
			{
				Task.IsPartOneDone = true;
			}
			public void FinishPartTwo()
			{
				Task.IsPartTwoDone = true;
			}
			public void FinishPartTree()
			{
				Task.IsPartTreeDone = true;
			}
		}
	}
	record MultipartTaskClaimArgs(MultipartTask task, MultipartTask.MultipartTaskManager manager)
	{
		public MultipartTask Task { get; init; } = task;
		public MultipartTask.MultipartTaskManager Manager { get; init; } = manager;
	}

	[TestMethod]
	public void TestLambdaArguments()
	{
		ClaimEventDispatcher<MultipartTask, MultipartTaskClaimArgs> taskDeliveredEvent = new();

		// Student
		taskDeliveredEvent += new ClaimEventListener<MultipartTask, MultipartTaskClaimArgs>()
		{
			OnCorroborate = (MultipartTask task) => task.Kind == "Homework",
			OnAccepted = (MultipartTaskClaimArgs args) =>
			{
				Console.WriteLine("Doing my homework...");
				args.manager.FinishPartOne();
				args.manager.FinishPartTwo();
				args.manager.FinishPartTree();
			}
		};

		// Baker 1
		taskDeliveredEvent += new ClaimEventListener<MultipartTask, MultipartTaskClaimArgs>()
		{
			OnCorroborate = (MultipartTask task) => task.Kind == "Bake",
			OnAccepted = (MultipartTaskClaimArgs args) =>
			{
				Console.WriteLine("Baking part 1...");
				args.manager.FinishPartOne();
			}
		};

		// Baker 3
		taskDeliveredEvent += new ClaimEventListener<MultipartTask, MultipartTaskClaimArgs>()
		{
			OnCorroborate = (MultipartTask task) => task.Kind == "Bake",
			OnAccepted = (MultipartTaskClaimArgs args) =>
			{
				Console.WriteLine("Baking part 2...");
				args.manager.FinishPartTree();
			}
		};

		// Baker 2
		taskDeliveredEvent += new ClaimEventListener<MultipartTask, MultipartTaskClaimArgs>()
		{
			OnCorroborate = (MultipartTask task) => task.Kind == "Bake",
			OnAccepted = (MultipartTaskClaimArgs args) =>
			{
				Console.WriteLine("Baking part 2...");
				args.manager.FinishPartTwo();
			}
		};

		var multipartTask = new MultipartTask("Bake");
		taskDeliveredEvent.Invoke(multipartTask, new Func<MultipartTaskClaimArgs>(() => new(multipartTask, new MultipartTask.MultipartTaskManager(multipartTask))) );
	}
	#endregion
}