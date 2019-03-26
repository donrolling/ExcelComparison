namespace Common.Transactions {
	public enum StatusDetail {
		New,
		Duplicate,
		Error,
		ItemNotFound,
		Unknown,
		Unauthorized,
		Invalid,
		ItemHasChildren,
		OK,
		Cancelled,
		APIOverage,
		Aborted,
		MixedFailureAndCancellation
	}
}