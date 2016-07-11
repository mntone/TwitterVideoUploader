using MediaFoundation;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class AudioMediaTypeExtension
	{
		public static int GetChannel(this IMFMediaType mediaType)
		{
			System.Diagnostics.Debug.Assert(mediaType.GetIsAudio());

			int channels;
			var hr = mediaType.GetUINT32(MFAttributesClsid.MF_MT_AUDIO_NUM_CHANNELS, out channels);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return channels;
		}

		public static int GetAudioProfileLevelIndication(this IMFMediaType mediaType)
		{
			System.Diagnostics.Debug.Assert(mediaType.GetIsAudio());
			System.Diagnostics.Debug.Assert(mediaType.GetSubType() == MFMediaType.AAC);

			int audioProfileLevelIndication;
			var hr = mediaType.GetUINT32(MFAttributesClsid.MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION, out audioProfileLevelIndication);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
			return audioProfileLevelIndication;
		}

		public static int GetAacObjectType(this IMFMediaType mediaType)
		{
			System.Diagnostics.Debug.Assert(mediaType.GetIsAudio());
			System.Diagnostics.Debug.Assert(mediaType.GetSubType() == MFMediaType.AAC);
			
			var blob = mediaType.GetBlob(MFAttributesClsid.MF_MT_USER_DATA);
			System.Diagnostics.Debug.Assert(blob.Length >= 12 /* HEAACWAVEINFO */ + 2);

			var objectType = 0x1f & (blob[12] >> 3);
			if (objectType == 0x1f)
			{
				objectType = 32 + (((0x7 & blob[12]) << 3) | (0x7 & (blob[13] >> 5)));
			}
			return objectType;
		}
	}
}
