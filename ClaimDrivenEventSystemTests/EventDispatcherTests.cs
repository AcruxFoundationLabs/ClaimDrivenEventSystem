using Acrux.CDES;
namespace CDES.Tests;

[TestClass]
public class EventDispatcherTests
{
	class CommandSentData
	{
		public string[] Keywords { get; init; }
	}

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
	public void TestSingleGeneric()
	{
		// Dispatcher creation
		EventDispatcher<CommandSentData> onCommandSent = new();

		// Listener 1
		EventListener<CommandSentData> printListener = new();
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
		EventListener<CommandSentData> sumListener = new();
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

	[TestMethod]
	public void TestMultipleGeneric()
	{
		EventDispatcher<Task, (Task task, Task.TaskManager manager)> taskDeliveredEvent = new();

		// Student
		EventListener<Task, (Task task, Task.TaskManager manager)> listener1 = new()
		{
			OnCorroborate = (Task task) => task.Kind == "Homework",
			OnAccepted = ((Task task, Task.TaskManager manager) args) =>
			{
				Console.WriteLine("Doing my homework...");
				args.manager.MarkAsDone();
			}
		};
		taskDeliveredEvent.Add(listener1);

		// Baker
		EventListener<Task, (Task task, Task.TaskManager manager)> listener2 = new()
		{
			OnCorroborate = (Task task) => task.Kind == "Bake",
			OnAccepted = ((Task task, Task.TaskManager manager) args) =>
			{
				Console.WriteLine("Baking...");
				args.manager.MarkAsDone();
			}
		};
		taskDeliveredEvent.Add(listener2);

		(Task task, Task.TaskManager manager) = Task.New("Bake");
		taskDeliveredEvent.Invoke(task, (task, manager));
	}
}