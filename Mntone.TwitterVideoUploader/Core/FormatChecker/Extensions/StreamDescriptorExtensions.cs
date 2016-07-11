using MediaFoundation;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class StreamDescriptorExtensions
	{
		public static IMFMediaTypeHandler GetMediaTypeHandler(this IMFStreamDescriptor streamDescriptor)
		{
			IMFMediaTypeHandler mediaTypeHandler;
			var hr = streamDescriptor.GetMediaTypeHandler(out mediaTypeHandler);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return mediaTypeHandler;
		}
	}
}