namespace Common.Transactions {
	public class TransactionMetaData {
		public ActionType ActionType { get; set; }

		public int ErrorCode { get; set; } = 0;

		public Status Status { get; set; }

		public StatusDetail StatusDetail { get; set; }
	}
}