namespace Business.Enums {
	public enum SignInStatus {
		Success,
		Failure,
		Error,
		Locked,
		NotFound,
		InActive,
		Username_Invalid,
		Password_Invalid,
		Success_WithTempPassword,
		AuthToken_Invalid,
		AuthenticationMethod_Invalid
	}
}