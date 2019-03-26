namespace Models.Base {
	public interface IAssociation<T, U> where T : struct {
		T FK_One { get; set; }
		U FK_Two { get; set; }
	}
}