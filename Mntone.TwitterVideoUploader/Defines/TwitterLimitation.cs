namespace Mntone.TwitterVideoUploader.Defines
{
	internal static class TwitterLimitation
	{
		public const long MaxFilesize = 512 * 1024 * 1024;

		public const long MaxDuration = 140 * 1000 * 1000 * 10; // 100 us

		public const double MinLength = 0.5; // sec
		public const double MaxLength = 140.0; // sec

		public const uint MaxFilesizeInMegabytes = 512; // MB

		public const uint MaxVideoBitrate = 3 * 1024 * 1024;

		public const uint MinWidth = 32;
		public const uint MinHeight = 32;
		public const uint MaxWidth = 1280;
		public const uint MaxHeight = 1024;

		public const double MinAspectRatio = 1.0 / 3.0;
		public const double MaxAspectRatio = 3.0;

		public const double MaxFramerate = 40.0;
	}
}