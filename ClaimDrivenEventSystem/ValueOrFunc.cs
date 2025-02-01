using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrux.EventSystems.ClaimDriven;

public class ValueOrFunc<T> : Either<T, Func<T>>
{
	public static implicit operator ValueOrFunc<T>(T value) => new(value);

	public static implicit operator ValueOrFunc<T>(Func<T> func) => new(func);

	public static ValueOrFunc<T> AsValue(T first)
			=> new ValueOrFunc<T>(first);

	public static ValueOrFunc<T> AsDelegate(Func<T> second)
		=> new ValueOrFunc<T>(second);

	protected ValueOrFunc(T value) : base(value) { }
	protected ValueOrFunc(Func<T> func) : base(func) { }
	private ValueOrFunc() { }
}
