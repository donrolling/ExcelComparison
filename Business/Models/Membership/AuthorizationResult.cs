namespace Business.Models.Membership {
	public class AuthorizationResult {
		public AuthorizationResultDetail AuthorizationResultDetail {
			get {
				return _authorizationResultDetail;
			}
			set {
				_authorizationResultDetail = value;
			}
		}
		public bool Authorized {
			get {
				return AuthorizationResultDetail == AuthorizationResultDetail.Success ? true : false;
			}
		}
		public string RedirectUrl { get; set; }
		private AuthorizationResultDetail _authorizationResultDetail = AuthorizationResultDetail.Success;

		public AuthorizationResult(AuthorizationResultDetail authorizationResultDetail) {
			this.AuthorizationResultDetail = authorizationResultDetail;
		}

		public AuthorizationResult(AuthorizationResultDetail authorizationResultDetail, string redirectUrl) {
			this.AuthorizationResultDetail = authorizationResultDetail;
			this.RedirectUrl = redirectUrl;
		}
	}
}