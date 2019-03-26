using Models.Interfaces;
using System.Collections.Generic;

namespace Models {
	public class DataResult<T> : IDataResult<T> {
		public IEnumerable<T> Data { get; private set; }
		public int Diplayed { get; private set; }
		public int Total { get; private set; }

		public DataResult(IEnumerable<T> data, int displayed, int total) {
			this.Data = data == null ? new List<T>() : data;
			this.Diplayed = displayed;
			this.Total = total;
		}
	}
}