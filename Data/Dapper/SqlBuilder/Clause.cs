namespace Data.Repository.Dapper.Base {
	public class Clause {
		public bool IsInclusive { get; set; }
		public object Parameters { get; set; }
		public string Sql { get; set; }
	}
}