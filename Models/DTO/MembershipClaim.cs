namespace Models.DTO
{
	public class MembershipClaim
	{
		public string Type { get; set; }

		public string Value { get; set; }

		public MembershipClaim(string navigationSection, string action)
		{
			Type = navigationSection;
			Value = action;
		}
	}
}