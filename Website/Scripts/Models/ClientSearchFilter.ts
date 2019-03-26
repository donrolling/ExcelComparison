class ClientSearchFilter {
	public ConditionType: string = "AND";
	public EqualityType: string = "EQUALS";
	public Name: string;
	public SearchLeftSide: boolean = false;
	public SearchRightSide: boolean = true;
	public Type: string = "string";
	public Value: object;
}