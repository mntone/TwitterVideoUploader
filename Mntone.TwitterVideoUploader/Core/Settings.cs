using Newtonsoft.Json;

namespace Mntone.TwitterVideoUploader.Core
{
	[JsonObject]
	public sealed class Settings
	{
		[JsonProperty(PropertyName = "access_token")]
		public string AccessToken { get; set; }

		[JsonProperty(PropertyName = "access_secret")]
		public string AccessSecret { get; set; }

		[JsonProperty(PropertyName = "screen_name")]
		public string ScreenName { get; set; }
	}
}