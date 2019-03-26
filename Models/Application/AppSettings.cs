namespace Models.Application
{
	public class AppSettings
	{
		public string BuildNumber { get; set; }

		public ConnectionStrings ConnectionStrings { get; set; }

		public FeatureToggles FeatureToggles { get; set; }

		public string TestLogin { get; set; }
		public string JWTSecret { get; set; }
	}
}