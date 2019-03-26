using Common.Transactions;

namespace Common.Models {
	/// <summary>
	/// This class id designed to represent basic Success/Failure with a message.
	/// It should be used to replace things like void actions. We almost always need a result for an action and this is the most basic.
	/// This class is inherited in places where more detail is required.
	/// </summary>
	public class MethodResult {
		public bool Failure {
			get { return _failure; }
			set {
				_failure = value;
				//this is here to make the success and failure values agree with one another
				_success = !_failure;
				//this is here to prevent the status to disagree with success and failure settings
				if (_failure && _status == Status.Success) {
					_status = Status.Failure;
				}
			}
		}

		public string Message { get; set; }

		public Status Status {
			get { return _status; }
			set { _status = value; }
		}

		public bool Success {
			get { return _success; }
			set {
				_success = value;
				//this is here to make the success and failure values agree with one another
				_failure = !_success;
				//this is here to prevent the status to disagree with success and failure settings
				if (!_success && _status == Status.Success) {
					_status = Status.Failure;
				}
			}
		}

		private bool _failure = false;
		private Status _status = Status.Success;
		private bool _success = true;

		public MethodResult() {
		}

		public MethodResult(bool success, string message) {
			this.Success = success;
			this.Message = message;
		}

		public MethodResult(bool success, string message, Status status) {
			this.Success = success;
			this.Message = message;
			this.Status = status;
		}

		public static MethodResult Fail(string message, Status status = Status.Failure) {
			return new MethodResult(false, message, status);
		}

		public static MethodResult Fail(Status status = Status.Failure) {
			return new MethodResult(false, string.Empty, status);
		}

		public static MethodResult Ok(Status status = Status.Success, string message = "") {
			return new MethodResult(true, message, status);
		}
	}
}