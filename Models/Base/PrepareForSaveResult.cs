namespace Models.Base {
	public class PrepareForSaveResult {
		public bool IsNew { get; set; }
		public bool IsValid { get; set; }
		public string ValidationMessage { get; set; }
	}
}