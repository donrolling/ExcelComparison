using Models.Application;

namespace Website.Models
{
	public class ClientAppSettings
	{
		public string BuildNumber { get; set; }

		public FeatureToggles FeatureToggles { get; set; }
		public string Login { get; internal set; }
	}
}