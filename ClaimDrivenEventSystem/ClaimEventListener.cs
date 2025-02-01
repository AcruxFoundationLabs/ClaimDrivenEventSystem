namespace Acrux.EventSystems.ClaimDriven;

/// <summary>
/// Manages the behaviour taken in the invokation of an event from a <see cref="ClaimEventDispatcher{TArgs}"/>.<br></br>
/// </summary>
/// <typeparam name="TCorroborateArgs"></typeparam>
/// <typeparam name="TClaimArgs"></typeparam>
public class ClaimEventListener<TCorroborateArgs, TClaimArgs>
{
	/// <summary>
	/// Defines the signature for the <see cref="OnCorroborate"/> property.
	/// </summary>
	/// <param name="args">The arguments of the raised dispatcher.</param>
	/// <returns>A <see cref="bool"/> indicating if this listener "claims" the event.</returns>
	public delegate bool CorroborateDelegate(TCorroborateArgs args);

	/// <summary>
	/// Defines the signature for the <see cref="OnAccepted"/> property.
	/// </summary>
	/// <param name="args">The arguments of the raised dispatcher.</param>
	public delegate void AcceptedDelegate(TClaimArgs args);

	/// <summary>
	/// Used to determinate if this listener will "claim" the event invokation to realize
	/// its defined behaviour, or if it "rejects" it.
	/// </summary>
	public CorroborateDelegate? OnCorroborate { get; set; }

	/// <summary>
	/// The behaviour of this listener that will be executed if the event invokation is "claimed".
	/// </summary>
	public AcceptedDelegate? OnAccepted { get; set; }

	/// <summary>
	/// Used to define the order of corroborance in a <see cref="ClaimEventDispatcher{TArgs}"/>.
	/// The lower the value, the more priority.
	/// </summary>
	public byte Priority
	{
		get => _priority;
		set
		{
			_priority = value;
			foreach(var dispatcher in Dispatchers)
			{
				dispatcher.ReorderListeners();
			}
		}
	}
	private byte _priority;

	internal List<ClaimEventDispatcher<TCorroborateArgs, TClaimArgs>> Dispatchers { get; } = [];
}

/// <summary>
/// Manages the behaviour taken in the invokation of an event from a <see cref="ClaimEventDispatcher{TArgs}"/>.<br></br>
/// </summary>
/// <typeparam name="TArgs"></typeparam>
public class ClaimEventListener<TArgs> : ClaimEventListener<TArgs, TArgs>
{

}