namespace CDES;

/// <summary>
/// Manages the invokation of an event, where different <see cref="EventListener{TArgs}"/> are attached to in.<br></br>
/// When invoked the handler asks the listeners one by one in order if they want to "claim" the event or "reject" it.<br></br>
/// After a listener "claims" an event, its functionality is invoked and the dispatch finishes.
/// </summary>
/// <typeparam name="TArgs"></typeparam>
public class EventDispatcher<TArgs>
{
	private List<EventListener<TArgs>> Listeners { get; } = [];

	/// <summary>
	/// Notifies all <see cref="Listeners"/> in order to "claim" the event and
	/// execute the claimer behaviour.
	/// </summary>
	/// <param name="args">The arguments of the event raise.</param>
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

	/// <summary>
	/// Attaches an <see cref="EventListener{TArgs}"/> to this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="EventListener{TArgs}"/> to attach.</param>
	public void Add(EventListener<TArgs> listener)
	{
		if (Listeners.Contains(listener)) return;

		Listeners.Add(listener);
		listener.Dispatchers.Add(this);
		ReorderListeners();
	}

	/// <summary>
	/// Detaches an <see cref="EventListener{TArgs}"/> from this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="EventListener{TArgs}"/> to attach.</param>
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
