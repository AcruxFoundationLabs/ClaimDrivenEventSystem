using System.ComponentModel;

namespace Acrux.CDES;

/// <summary>
/// Manages the invokation of an event, where different <see cref="ClaimEventListener{TArgs}"/> are attached to in.<br></br>
/// When invoked the handler asks the listeners one by one in order if they want to "claim" the event or "reject" it.<br></br>
/// After a listener "claims" an event, its functionality is invoked and the dispatch finishes.
/// </summary>
/// <typeparam name="TCorroborateArgs">The values handed in to the listener to corroborate the claim</typeparam>
/// /// <typeparam name="TClaimArgs">The values handed in to the listener for the claimed behaviour</typeparam>
public class ClaimEventDispatcher<TCorroborateArgs, TClaimArgs>
{
	private List<ClaimEventListener<TCorroborateArgs, TClaimArgs>> Listeners { get; } = [];

	public static ClaimEventDispatcher<TCorroborateArgs, TClaimArgs> operator +(ClaimEventDispatcher<TCorroborateArgs, TClaimArgs> dispatcher, ClaimEventListener<TCorroborateArgs, TClaimArgs> listener)
	{
		dispatcher.Add(listener);
		return dispatcher;
	}

	public static ClaimEventDispatcher<TCorroborateArgs, TClaimArgs> operator -(ClaimEventDispatcher<TCorroborateArgs, TClaimArgs> dispatcher, ClaimEventListener<TCorroborateArgs, TClaimArgs> listener)
	{
		dispatcher.Remove(listener);
		return dispatcher;
	}

	/// <summary>
	/// Notifies all <see cref="Listeners"/> in order to "claim" the event and
	/// execute the claimer behaviour.
	/// </summary>
	/// <param name="corroborateArgs">The arguments of the event raise handed to the listener to decide if claiming will be done.</param>
	/// <param name="claimArgs">The arguments of the event handed-in for the claimed behaviour.</param>
	public void Invoke(Either<TCorroborateArgs, Func<TCorroborateArgs>> corroborateArgs, Either<TClaimArgs, Func<TClaimArgs>> claimArgs)
	{
		Console.WriteLine("Dispatching...");
		if (Listeners.Count == 0)
		{
			Console.WriteLine("Dispatch canceled, no listeners attached.");
			return;
		}

		Console.WriteLine("Corroborating Listeners...");
		ClaimEventListener<TCorroborateArgs, TClaimArgs>? suitableListener = null;
		foreach (var listener in Listeners)
		{
			TCorroborateArgs _corroborateArgs = corroborateArgs.IsFirst ? corroborateArgs.First : corroborateArgs.Second.Invoke();
			if (listener.OnCorroborate?.Invoke(_corroborateArgs) ?? false)
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

		TClaimArgs _claimArgs = claimArgs.IsFirst ? claimArgs.First : claimArgs.Second.Invoke();
		Console.WriteLine("Calling Listener handler...");
		suitableListener?.OnAccepted?.Invoke(_claimArgs);
		Console.WriteLine("Listener handler called.\nDispatched.\n");
	}

	/// <summary>
	/// Attaches an <see cref="ClaimEventListener{TArgs}"/> to this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="ClaimEventListener{TArgs}"/> to attach.</param>
	public void Add(ClaimEventListener<TCorroborateArgs, TClaimArgs> listener)
	{
		if (Listeners.Contains(listener)) return;

		Listeners.Add(listener);
		listener.Dispatchers.Add(this);
		ReorderListeners();
	}

	/// <summary>
	/// Detaches an <see cref="ClaimEventListener{TArgs}"/> from this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="ClaimEventListener{TArgs}"/> to attach.</param>
	public void Remove(ClaimEventListener<TCorroborateArgs, TClaimArgs> listener)
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
/// Manages the invokation of an event, where different <see cref="ClaimEventListener{TArgs}"/> are attached to in.<br></br>
/// When invoked the handler asks the listeners one by one in order if they want to "claim" the event or "reject" it.<br></br>
/// After a listener "claims" an event, its functionality is invoked and the dispatch finishes.
/// </summary>
/// <typeparam name="TCorroborateArgs">The values handed in to the listener.</typeparam>
public class ClaimEventDispatcher<TArgs> : ClaimEventDispatcher<TArgs, TArgs>
{
	public void Invoke(Either<TArgs, Func<TArgs>> args)
	{
		Invoke(args, args);
	}
}