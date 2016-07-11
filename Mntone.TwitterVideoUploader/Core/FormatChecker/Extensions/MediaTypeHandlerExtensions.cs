using MediaFoundation;
using System;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class MediaTypeHandlerExtension
	{
		public static Guid GetMajorType(this IMFMediaTypeHandler mediaTypeHandler)
		{
			Guid majorType;
			var hr = mediaTypeHandler.GetMajorType(out majorType);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return majorType;
		}

		public static IMFMediaType GetMediaType(this IMFMediaTypeHandler mediaTypeHandler)
		{
			IMFMediaType mediaType;
			var hr = mediaTypeHandler.GetCurrentMediaType(out mediaType);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return mediaType;
		}
	}
}
