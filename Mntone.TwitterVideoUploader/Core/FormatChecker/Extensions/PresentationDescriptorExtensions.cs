using MediaFoundation;
using System.Text;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class PresentationDescriptorExtensions
	{
		public static int GetStreamDescriptorCount(this IMFPresentationDescriptor presentationDescriptor)
		{
			int streams;
			var hr = presentationDescriptor.GetStreamDescriptorCount(out streams);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return streams;
		}

		public static IMFStreamDescriptor GetStreamDescriptorByIndex(this IMFPresentationDescriptor presentationDescriptor, int index)
		{
			bool selected;
			IMFStreamDescriptor streamDescriptor;
			var hr = presentationDescriptor.GetStreamDescriptorByIndex(index, out selected, out streamDescriptor);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return streamDescriptor;
		}

		public static string GetMineType(this IMFPresentationDescriptor presentationDescriptor)
		{
			int length;
			var hr = presentationDescriptor.GetStringLength(MFAttributesClsid.MF_PD_MIME_TYPE, out length);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			var mineTypeBuilder = new StringBuilder(length + 1);
			hr = presentationDescriptor.GetString(MFAttributesClsid.MF_PD_MIME_TYPE, mineTypeBuilder, length + 1, out length);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return mineTypeBuilder.ToString();
		}

		public static long GetDuration(this IMFPresentationDescriptor presentationDescriptor)
		{
			long duration;
			var hr = presentationDescriptor.GetUINT64(MFAttributesClsid.MF_PD_DURATION, out duration);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return duration;
		}

		public static int GetVideoEncodingBitrate(this IMFPresentationDescriptor presentationDescriptor)
		{
			int duration;
			var hr = presentationDescriptor.GetUINT32(MFAttributesClsid.MF_PD_VIDEO_ENCODING_BITRATE, out duration);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return duration;
		}

		public static int GetAudioEncodingBitrate(this IMFPresentationDescriptor presentationDescriptor)
		{
			int duration;
			var hr = presentationDescriptor.GetUINT32(MFAttributesClsid.MF_PD_AUDIO_ENCODING_BITRATE, out duration);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return duration;
		}
	}
}
