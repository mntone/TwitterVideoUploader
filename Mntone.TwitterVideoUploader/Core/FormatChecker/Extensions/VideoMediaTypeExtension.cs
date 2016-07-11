using MediaFoundation;
using Mntone.TwitterVideoUploader.Core.FormatChecker.Units;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class VideoMediaTypeExtension
	{
		public static Size GetSize(this IMFMediaType mediaType)
		{
			System.Diagnostics.Debug.Assert(mediaType.GetIsVideo());

			int width, height;
			var hr = MFExtern.MFGetAttributeSize(mediaType, MFAttributesClsid.MF_MT_FRAME_SIZE, out width, out height);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			return new Size(width, height);
		}

		public static Fraction GetFramerate(this IMFMediaType mediaType)
		{
			System.Diagnostics.Debug.Assert(mediaType.GetIsVideo());

			long framerate;
			var hr = mediaType.GetUINT64(MFAttributesClsid.MF_MT_FRAME_RATE, out framerate);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			return Fraction.FromUInt64(framerate);
		}

		public static Fraction GetPixelAspectRatio(this IMFMediaType mediaType)
		{
			System.Diagnostics.Debug.Assert(mediaType.GetIsVideo());

			long pixelAspectRatio;
			var hr = mediaType.GetUINT64(MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, out pixelAspectRatio);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			return Fraction.FromUInt64(pixelAspectRatio);
		}

		public static MFVideoInterlaceMode GetInterlaceMode(this IMFMediaType mediaType)
		{
			System.Diagnostics.Debug.Assert(mediaType.GetIsVideo());

			int interlaceMode;
			var hr = mediaType.GetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, out interlaceMode);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			return (MFVideoInterlaceMode)interlaceMode;
		}
	}
}
