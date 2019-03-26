using System;
using System.Collections.Generic;

namespace Models.Interfaces {
	public interface IDataResult<T> {
		IEnumerable<T> Data { get; }
		int Diplayed { get; }
		int Total { get; }
	}
}