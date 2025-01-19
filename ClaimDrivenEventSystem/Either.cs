using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrux.CDES
{
	/// <summary>
	/// Stores a value whose type is either <typeparamref name="T1"/>
	/// or <typeparamref name="T2"/>
	/// </summary>
	/// <typeparam name="T1">The first possible type</typeparam>
	/// <typeparam name="T2">The second possible type</typeparam>
	public class Either<T1, T2>
	{
		public T1? First { get; }
		public T2? Second { get; }
		public bool IsFirst { get; }


		public static implicit operator Either<T1, T2>(T1 value) => new(value);

		public static implicit operator Either<T1, T2>(T2 value) => new(value);

		public static Either<T1,T2> AsFirst(T1 first)
			=> new Either<T1,T2>(first);

		public static Either<T1, T2> AsSecond(T2 second)
			=> new Either<T1, T2>(second);

		protected Either(T1 value)
		{
			IsFirst = true;
			First = value;
		}

		protected Either(T2 value)
		{
			IsFirst = false;
			Second = value;
		}

		protected Either() { }
	}
}
