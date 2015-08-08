using static CoreTweet.OAuth;

namespace Mntone.TwitterVideoUploader.Views
{
	public sealed class AuthorizationSessionData
	{
		public OAuthSession Session { get; set; } = null;
		public uint PinCode { get; set; } = 0;
	}
}