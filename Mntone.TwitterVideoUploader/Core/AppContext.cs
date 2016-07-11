using CoreTweet;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using static CoreTweet.OAuth;

namespace Mntone.TwitterVideoUploader.Core
{
	public sealed class AppContext
	{
		private const string consumerKey = "3zepzfZr2jSSzT9bpZqX4cT9M";
		private const string consumerSecret = "epLP6WxmuOo7DLeC21WjagDeBVn4TbKACb3lmuLPBTDKGMVojS";

		private const string settingsFileName = "./settings.json";

		private bool _isDirty = false;

		public Tokens Tokens { get; private set; } = null;
		public bool IsTokensLoaded { get; private set; } = false;

		public AppContext()
		{ }

		public async Task LoadSettingsAsync()
		{
			if (!File.Exists(settingsFileName)) return;

			Settings settings;
			using (var stream = File.Open(settingsFileName, FileMode.Open))
			using (var reader = new StreamReader(stream))
			{
				var jsonData = await reader.ReadToEndAsync();
				settings = JsonConvert.DeserializeObject<Settings>(jsonData);
			}
			this.Tokens = Tokens.Create(consumerKey, consumerSecret, settings.AccessToken, settings.AccessSecret, screenName: settings.ScreenName);
			this.IsTokensLoaded = true;
		}

		public async Task SaveSettingsAsync()
		{
			if (this.Tokens == null) return;
			if (!this._isDirty) return;

			var settings = new Settings()
			{
				AccessToken = this.Tokens.AccessToken,
				AccessSecret = this.Tokens.AccessTokenSecret,
				ScreenName = this.Tokens.ScreenName,
			};
			var jsonData = JsonConvert.SerializeObject(settings);
			using (var stream = File.Open(settingsFileName, FileMode.Create))
			using (var writer = new StreamWriter(stream))
			{
				await writer.WriteLineAsync(jsonData);
			}
		}

		public Task<OAuthSession> AuthorizeAsync()
		{
			return OAuth.AuthorizeAsync(consumerKey, consumerSecret);
		}

		public async Task SetTokensFromPinCodeAsync(OAuthSession session, uint pinCode)
		{
			this.Tokens = new Tokens(await OAuth.GetTokensAsync(session, pinCode.ToString()));
			this.IsTokensLoaded = true;
			this._isDirty = true;
		}

		public async Task<StatusResponse> UploadAsync(string status, string filename)
		{
			if (!this.IsTokensLoaded)
			{
				throw new InvalidOperationException("Not sign in!");
			}

			using (var stream = new FileStream(filename, FileMode.Open))
			{
				var mediaResult = await this.Tokens.Media.UploadChunkedAsync(stream, UploadMediaType.Video);
				var postResult = await this.Tokens.Statuses.UpdateAsync(status, media_ids: new long[] { mediaResult.MediaId });
				return postResult;
			}
		}
	}
}
