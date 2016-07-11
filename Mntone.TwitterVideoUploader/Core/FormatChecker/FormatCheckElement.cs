namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	public enum FormatCheckElement : byte
	{
		Existence,
		Filesize,
		MineType,
		Duration,

		VideoCodec = 100,
		VideoBitrate,
		VideoWidth,
		VideoHeight,
		VideoAspectRatio,
		VideoFramerate,
		VideoPixelAspectRatio,
		VideoInterlaceMode,

		AudioCodec = 200,
		AudioAacObjectType,
		AudioChannel,
	}
}