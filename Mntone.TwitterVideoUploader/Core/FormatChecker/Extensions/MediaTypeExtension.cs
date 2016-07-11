using MediaFoundation;
using System;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class MediaTypeExtension
	{
		public static Guid GetMajorType(this IMFMediaType mediaType)
		{
			Guid majorType;
			var hr = mediaType.GetMajorType(out majorType);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return majorType;
		}
		public static bool GetIsVideo(this IMFMediaType mediaType) => mediaType.GetMajorType() == MFMediaType.Video;
		public static bool GetIsAudio(this IMFMediaType mediaType) => mediaType.GetMajorType() == MFMediaType.Audio;

		public static Guid GetSubType(this IMFMediaType mediaType)
		{
			Guid subType;
			var hr = mediaType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out subType);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return subType;
		}

		public static byte[] GetBlob(this IMFMediaType mediaType, Guid guidKey)
		{
			int size;
			var hr = mediaType.GetBlobSize(guidKey, out size);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			var blob = new byte[size];
			hr = mediaType.GetBlob(guidKey, blob, size, out size);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return blob;
		}

		public static int GetAverageBitrate(this IMFMediaType mediaType)
		{
			int averageBitrate;
			var hr = mediaType.GetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, out averageBitrate);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return averageBitrate;
		}
	}
}
