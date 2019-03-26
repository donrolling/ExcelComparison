namespace Data.Dapper.Models {
	public class SearchFilterDTO {
		public string ConditionType { get; set; }
		public string EqualityType { get; set; }
		public string Name { get; set; }
		public bool SearchLeftSide { get; set; } = false;
		public bool SearchRightSide { get; set; } = true;
		public string Type { get; set; }
		public object Value { get; set; }
	}
}