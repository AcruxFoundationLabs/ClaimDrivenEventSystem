namespace CDES;

public class EventDispatcher<TArgs>
{
	private List<EventListener<TArgs>> Listeners { get; } = [];

	public void Invoke(TArgs args)
	{
		Console.WriteLine("Dispatching...");
		if (Listeners.Count == 0)
		{
			Console.WriteLine("Dispatch canceled, no listeners attached.");
			return;
		}

		Console.WriteLine("Corroborating Listeners...");
		EventListener<TArgs>? suitableListener = null;
		foreach (var listener in Listeners)
		{
			if (listener.OnCorroborate?.Invoke(args) ?? false)
			{
				Console.WriteLine("Listener claimed the handling of this dispatch.");
				suitableListener = listener;
				break;
			}
			Console.WriteLine("Listener declined the handling of this dispatch.");
		}

		if (suitableListener == null)
		{
			Console.WriteLine("Dispatch terminated. no listener claimed the handle of this dispatch.\n");
			return;
		}

		Console.WriteLine("Calling Listener handler...");
		suitableListener?.OnAccepted?.Invoke(args);
		Console.WriteLine("Listener handler called.\nDispatched.\n");
	}

	public void Add(EventListener<TArgs> listener)
	{
		if (Listeners.Contains(listener)) return;

		Listeners.Add(listener);
		listener.Dispatchers.Add(this);
		ReorderListeners();
	}

	public void Remove(EventListener<TArgs> listener)
	{
		if (!Listeners.Contains(listener)) return;

		Listeners.Remove(listener);
		listener.Dispatchers.Remove(this);
	}

	internal void ReorderListeners()
	{
		Listeners.OrderBy(x => x.Priority);
	}
}
