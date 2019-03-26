using System;

namespace Business.Models.Membership {
	public class LoginStatus {
		public Guid Id { get; set; }
		public string Message { get; set; }
		public SignInStatus Status { get; set; }

		public static LoginStatus GetLoginStatus(SignInStatus loginStatus) {
			return GetLoginStatus(loginStatus, Guid.Empty);
		}

		public static LoginStatus GetLoginStatus(SignInStatus loginStatus, Guid id) {
			var status = new LoginStatus { Status = loginStatus, Id = id };

			var message = string.Empty;
			switch (loginStatus) {
				case SignInStatus.Success:
					message = "Success.";
					break;

				case SignInStatus.Error:
					message = "Unknown error.";
					break;

				case SignInStatus.Failure:
				case SignInStatus.Password_Invalid:
				case SignInStatus.NotFound:
					message = "Username or password is not correct.";
					break;
			}
			status.Message = message;

			return status;
		}
	}
}