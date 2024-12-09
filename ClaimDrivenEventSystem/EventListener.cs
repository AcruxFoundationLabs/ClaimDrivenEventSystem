namespace CDES;

public class EventListener<TArgs>
{
	public delegate bool CorroborateDelegate(TArgs args);
	public delegate void AcceptedDelegate(TArgs args);

	public CorroborateDelegate? OnCorroborate { get; set; }
	public AcceptedDelegate? OnAccepted { get; set; }
	public byte Priority
	{
		get => _priority;
		set
		{
			_priority = value;
			Dispatcher?.ReorderListeners();
		}
	}
	private byte _priority;

	internal EventDispatcher<TArgs>? Dispatcher { get; set; }
}
