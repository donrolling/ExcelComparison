namespace Models.Base {
	public interface IAssociationTo3<T, U, V> where T : struct where U : struct {
		T FK_One { get; set; }
		V FK_Three { get; set; }
		U FK_Two { get; set; }
	}
}