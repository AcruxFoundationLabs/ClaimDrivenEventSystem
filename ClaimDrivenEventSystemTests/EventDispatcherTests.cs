using Acrux.CDES;
namespace CDES.Tests;

[TestClass]
public class EventDispatcherTests
{
	class CommandSentData
	{
		public string[] Keywords { get; init; }
	}

	[TestMethod]
	public void Test()
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
}