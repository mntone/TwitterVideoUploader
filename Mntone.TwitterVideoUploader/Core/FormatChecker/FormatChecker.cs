using MediaFoundation;
using Mntone.TwitterVideoUploader.Defines;
using System;
using System.Collections.Generic;

namespace Mntone.TwitterVideoUploader.Core.FormatChecker
{
	internal static class FormatChecker
	{
		public static void Initialize()
		{
			var hr = MFExtern.MFStartup(0x10070, MFStartup.Lite);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);
		}

		public static IReadOnlyDictionary<FormatCheckElement, Tuple<bool, object>> CheckFormat(string filename)
		{
			var ret = new Dictionary<FormatCheckElement, Tuple<bool, object>>();

			var existence = System.IO.File.Exists(filename);
			ret.Add(FormatCheckElement.Existence, Tuple.Create<bool, object>(existence, existence));
			if (!existence) return ret;

			var filesize = (new System.IO.FileInfo(filename)).Length;
			ret.Add(FormatCheckElement.Filesize, Tuple.Create<bool, object>(filesize <= TwitterLimitation.MaxFilesize, filesize));

			IMFSourceResolver sourceResolver;
			var hr = MFExtern.MFCreateSourceResolver(out sourceResolver);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			IMFMediaSource mediaSource;
			try
			{
				mediaSource = sourceResolver.GetMediaTypeHandlerAsMediaSource(filename);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				ret.Add(FormatCheckElement.MineType, Tuple.Create<bool, object>(false, "unknown"));
				return ret;
			}

			IMFPresentationDescriptor presDesc;
			hr = mediaSource.CreatePresentationDescriptor(out presDesc);
			MediaFoundation.Misc.MFError.ThrowExceptionForHR(hr);

			var mineType = presDesc.GetMineType();
			ret.Add(FormatCheckElement.MineType, Tuple.Create<bool, object>(mineType == "video/mp4", mineType));

			var duration = presDesc.GetDuration();
			ret.Add(FormatCheckElement.Duration, Tuple.Create<bool, object>(duration <= TwitterLimitation.MaxDuration, duration));

			var streams = presDesc.GetStreamDescriptorCount();
			for (int i = 0; i < streams; ++i)
			{
				var streamDescriptor = presDesc.GetStreamDescriptorByIndex(i);
				var mediaTypeHandler = streamDescriptor.GetMediaTypeHandler();
				var majorType = mediaTypeHandler.GetMajorType();
				if (majorType == MFMediaType.Video)
				{
					var mediaType = mediaTypeHandler.GetMediaType();
					CheckVideoFormat(mediaType, ret);
				}
				else if (majorType == MFMediaType.Audio)
				{
					var mediaType = mediaTypeHandler.GetMediaType();
					CheckAudioFormat(mediaType, ret);
				}
			}

			return ret;
		}

		private static void CheckVideoFormat(IMFMediaType mediaType, IDictionary<FormatCheckElement, Tuple<bool, object>> ret)
		{
			var subType = mediaType.GetSubType();
			ret.Add(FormatCheckElement.VideoCodec, Tuple.Create<bool, object>(subType == MFMediaType.H264, subType));

			var bitrate = mediaType.GetAverageBitrate();
			ret.Add(FormatCheckElement.VideoBitrate, Tuple.Create<bool, object>(bitrate < TwitterLimitation.MaxVideoBitrate, bitrate));

			var size = mediaType.GetSize();
			ret.Add(FormatCheckElement.VideoWidth, Tuple.Create<bool, object>(size.Width >= TwitterLimitation.MinWidth && size.Width <= TwitterLimitation.MaxWidth, size.Width));
			ret.Add(FormatCheckElement.VideoHeight, Tuple.Create<bool, object>(size.Height >= TwitterLimitation.MinHeight && size.Height <= TwitterLimitation.MaxHeight, size.Height));

			var aspectRatio = (double)size.Width / size.Height;
			ret.Add(FormatCheckElement.VideoAspectRatio, Tuple.Create<bool, object>(aspectRatio >= TwitterLimitation.MinAspectRatio && aspectRatio <= TwitterLimitation.MaxAspectRatio, aspectRatio));

			var framerateDouble = mediaType.GetFramerate().ToDouble();
			ret.Add(FormatCheckElement.VideoFramerate, Tuple.Create<bool, object>(framerateDouble <= TwitterLimitation.MaxFramerate, framerateDouble));

			var pixelAspectRatio = mediaType.GetPixelAspectRatio();
			ret.Add(FormatCheckElement.VideoPixelAspectRatio, Tuple.Create<bool, object>(pixelAspectRatio.Numerator == pixelAspectRatio.Denominator, pixelAspectRatio));

			var interlaceMode = mediaType.GetInterlaceMode();
			ret.Add(FormatCheckElement.VideoInterlaceMode, Tuple.Create<bool, object>(interlaceMode == MFVideoInterlaceMode.Progressive || interlaceMode == MFVideoInterlaceMode.MixedInterlaceOrProgressive, interlaceMode));
		}

		private static void CheckAudioFormat(IMFMediaType mediaType, IDictionary<FormatCheckElement, Tuple<bool, object>> ret)
		{
			var subType = mediaType.GetSubType();
			var isAac = subType == MFMediaType.AAC;
			ret.Add(FormatCheckElement.AudioCodec, Tuple.Create<bool, object>(isAac, subType));

			if (isAac)
			{
				var aacObjectType = mediaType.GetAacObjectType();
				ret.Add(FormatCheckElement.AudioAacObjectType, Tuple.Create<bool, object>(aacObjectType == 2, aacObjectType));

				var channels = mediaType.GetChannel();
				ret.Add(FormatCheckElement.AudioChannel, Tuple.Create<bool, object>(channels == 1 || channels == 2, channels));
			}
		}
	}
}