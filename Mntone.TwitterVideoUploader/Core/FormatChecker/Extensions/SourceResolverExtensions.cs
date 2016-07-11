using MediaFoundation;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class SourceResolverExtensions
	{
		public static IMFMediaSource GetMediaTypeHandlerAsMediaSource(this IMFSourceResolver sourceResolver, string url)
		{
			MFObjectType objectType;
			object source;
			var hr = sourceResolver.CreateObjectFromURL(url, MFResolution.MediaSource, null, out objectType, out source);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			System.Diagnostics.Debug.Assert(objectType == MFObjectType.MediaSource);
			return (IMFMediaSource)source;
		}
	}
}