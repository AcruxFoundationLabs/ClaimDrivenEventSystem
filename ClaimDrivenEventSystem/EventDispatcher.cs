namespace CDES;

/// <summary>
/// Manages the invokation of an event, where different <see cref="EventListener{TArgs}"/> are attached to in.<br></br>
/// When invoked the handler asks the listeners one by one in order if they want to "claim" the event or "reject" it.<br></br>
/// After a listener "claims" an event, its functionality is invoked and the dispatch finishes.
/// </summary>
/// <typeparam name="TCorroborateArgs">The values handed in to the listener to corroborate the claim</typeparam>
/// /// <typeparam name="TClaimArgs">The values handed in to the listener for the claimed behaviour</typeparam>
public class EventDispatcher<TCorroborateArgs, TClaimArgs>
{
	private List<EventListener<TCorroborateArgs, TClaimArgs>> Listeners { get; } = [];

	/// <summary>
	/// Notifies all <see cref="Listeners"/> in order to "claim" the event and
	/// execute the claimer behaviour.
	/// </summary>
	/// <param name="corroborateArgs">The arguments of the event raise handed to the listener to decide if claiming will be done.</param>
	/// <param name="claimArgs">The arguments of the event handed-in for the claimed behaviour.</param>
	public void Invoke(Func<TCorroborateArgs> corroborateArgs, Func<TClaimArgs> claimArgs)
	{
		Console.WriteLine("Dispatching...");
		if (Listeners.Count == 0)
		{
			Console.WriteLine("Dispatch canceled, no listeners attached.");
			return;
		}

		Console.WriteLine("Corroborating Listeners...");
		EventListener<TCorroborateArgs, TClaimArgs>? suitableListener = null;
		foreach (var listener in Listeners)
		{
			if (listener.OnCorroborate?.Invoke(corroborateArgs.Invoke()) ?? false)
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
		suitableListener?.OnAccepted?.Invoke(claimArgs.Invoke());
		Console.WriteLine("Listener handler called.\nDispatched.\n");
	}

	/// <summary>
	/// Attaches an <see cref="EventListener{TArgs}"/> to this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="EventListener{TArgs}"/> to attach.</param>
	public void Add(EventListener<TCorroborateArgs, TClaimArgs> listener)
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
	public void Remove(EventListener<TCorroborateArgs, TClaimArgs> listener)
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

/// <summary>
/// Manages the invokation of an event, where different <see cref="EventListener{TArgs}"/> are attached to in.<br></br>
/// When invoked the handler asks the listeners one by one in order if they want to "claim" the event or "reject" it.<br></br>
/// After a listener "claims" an event, its functionality is invoked and the dispatch finishes.
/// </summary>
/// <typeparam name="TCorroborateArgs">The values handed in to the listener.</typeparam>
public class EventDispatcher<TArgs> : EventDispatcher<TArgs, TArgs>
{
	public void Invoke(Func<TArgs> args)
	{
		Invoke(args, args);
	}
}