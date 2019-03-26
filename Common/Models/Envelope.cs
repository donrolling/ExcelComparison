using Common.Transactions;

namespace Common.Models {

	public class Envelope<T> : MethodResult {
		public T Result { get; set; }

		public Envelope() { }

		public Envelope(MethodResult methodResult) {
			this.Status = methodResult.Status;
			this.Success = methodResult.Success;
			this.Message = methodResult.Message;
		}

		public Envelope(MethodResult methodResult, T result) : this(methodResult) {
			this.Result = result;
		}

		public static Envelope<T> Ok(T result, string message = "") {
			var _base = MethodResult.Ok(Status.Success, message);
			return new Envelope<T>(_base, result);
		}

		public new static Envelope<T> Fail(string message, Status status = Status.Failure) {
			var _base = MethodResult.Fail(message);
			return new Envelope<T>(_base);
		}
	}
}