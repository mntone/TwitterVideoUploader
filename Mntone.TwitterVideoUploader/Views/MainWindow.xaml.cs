using CoreTweet;
using Microsoft.Win32;
using Mntone.TwitterVideoUploader.Core;
using Mntone.Windows.PerMonitorDpiSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mntone.TwitterVideoUploader.Views
{
	public partial class MainWindow : PerMonitorDpiSupportWindow, INotifyPropertyChanged
	{
		private static AppContext Ctx => ((App)App.Current).Context;

		public AuthorizationSessionData SessionData { get; private set; }

		public bool IsSignIn
		{
			get { return this._IsSignIn; }
			set
			{
				if (this._IsSignIn == value) return;

				this._IsSignIn = value;
				this.RaisePropertyChanged(IsUploadEnabledPropertyChangedEventArgs);
				this.RaisePropertyChanged(IsSignInPropertyChangedEventArgs);
				this.RaisePropertyChanged(IsNotSignInPropertyChangedEventArgs);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool _IsSignIn = false;

		public bool IsNotSignIn => !this._IsSignIn;

		private bool IsBusy
		{
			get { return this._IsBusy; }
			set
			{
				if (this._IsBusy == value) return;

				this._IsBusy = value;
				this.RaisePropertyChanged(IsUploadEnabledPropertyChangedEventArgs);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool _IsBusy = false;

		private bool IsValidFile
		{
			get { return this._IsValidFile; }
			set
			{
				if (this._IsValidFile == value) return;

				this._IsValidFile = value;
				this.RaisePropertyChanged(IsInvalidFilePropertyChangedEventArgs);
				this.RaisePropertyChanged(IsUploadEnabledPropertyChangedEventArgs);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool _IsValidFile = false;

		public bool IsInvalidFile => !this.IsValidFile;
		public bool IsUploadEnabled => this.IsSignIn && !this.IsBusy && this.IsValidFile;

		public string ErrorMessage
		{
			get { return this._ErrorMessage; }
			set
			{
				if (this._ErrorMessage == value) return;

				this._ErrorMessage = value;
				this.RaisePropertyChanged(ErrorMessagePropertyChangedEventArgs);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string _ErrorMessage = string.Empty;

		private CancellationTokenSource LazyEvaluationCancellationTokenSource { get; set; }


		public MainWindow()
		{
			this.InitializeComponent();
			this.EvaluteFile();
		}

		private void FilenameTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			this.EvaluteFile();
		}

		private async void EvaluteFile()
		{
			this.IsValidFile = false;

			if (this.LazyEvaluationCancellationTokenSource != null)
			{
				this.LazyEvaluationCancellationTokenSource.Dispose();
			}

			var filename = this.FilenameTextBox.Text;
			var cts = new CancellationTokenSource();
			this.LazyEvaluationCancellationTokenSource = cts;
			await Task.Factory.StartNew(() => Core.FormatChecker.FormatChecker.CheckFormat(filename), cts.Token).ContinueWith(prevTask =>
			{
				if (cts.IsCancellationRequested) return;

				var formatCheck = prevTask.Result;
				this.ErrorMessage = GenerateErrorMessage(formatCheck);
				this.IsValidFile = formatCheck.Values.All(s => s.Item1);
			}, TaskScheduler.Current);
		}

		private static string GenerateErrorMessage(IReadOnlyDictionary<Core.FormatChecker.FormatCheckElement, Tuple<bool, object>> formatCheck)
		{
			bool resFlag = false;
			var builder = new StringBuilder();
			foreach (var item in formatCheck)
			{
				if (item.Value.Item1) continue;

				switch (item.Key)
				{
					case Core.FormatChecker.FormatCheckElement.Existence:
						builder.AppendLine(Properties.Resources.FileDoesNotExist);
						break;

					case Core.FormatChecker.FormatCheckElement.Filesize:
						{
							var maxFilesizeInMegabytes = Defines.TwitterLimitation.MaxFilesize / 1024.0 / 1024.0;
							var filesizeInMegabytes = ((long)item.Value.Item2) / 1024.0 / 1024.0;
							builder.AppendLine(string.Format(Properties.Resources.FilesizeExceeded + "\t ({1:0.0} MB)", maxFilesizeInMegabytes, filesizeInMegabytes));
						}
						break;

					case Core.FormatChecker.FormatCheckElement.MineType:
						builder.AppendLine(string.Format(Properties.Resources.MineTypeIsIncorrect + "\t ({0})", item.Value.Item2));
						break;

					case Core.FormatChecker.FormatCheckElement.Duration:
						{
							var maxDurationInSeconds = Defines.TwitterLimitation.MaxDuration / 1000.0 / 1000.0 / 10.0;
							var durationInSeconds = ((long)item.Value.Item2) / 1000.0 / 1000.0 / 10.0;
							builder.AppendLine(string.Format(Properties.Resources.DurationExceeded + "\t ({1:0.00} sec)", maxDurationInSeconds, durationInSeconds));
						}
						break;

					case Core.FormatChecker.FormatCheckElement.VideoCodec:
						builder.AppendLine(Properties.Resources.VideoCodecIsNotH264Avc);
						break;

					case Core.FormatChecker.FormatCheckElement.VideoBitrate:
						{
							var maxVideoBitrateInMegabits = Defines.TwitterLimitation.MaxVideoBitrate / 1024.0 / 1024.0;
							var videoBitrateInMegabits = ((int)item.Value.Item2) / 1024.0 / 1024.0;
							builder.AppendLine(string.Format(Properties.Resources.VideoBitrateExceeded + "\t ({1:0.00} Mbps)", maxVideoBitrateInMegabits, videoBitrateInMegabits));
						}
						break;

					case Core.FormatChecker.FormatCheckElement.VideoWidth:
					case Core.FormatChecker.FormatCheckElement.VideoHeight:
						if (resFlag) continue;
						{
							var width = (int)formatCheck[Core.FormatChecker.FormatCheckElement.VideoWidth].Item2;
							var height = (int)formatCheck[Core.FormatChecker.FormatCheckElement.VideoHeight].Item2;
							builder.AppendLine(string.Format(Properties.Resources.ResolutionExceeded + "\t ({2}x{3})",
								Defines.TwitterLimitation.MaxWidth, Defines.TwitterLimitation.MaxHeight, width, height));
						}
						break;

					case Core.FormatChecker.FormatCheckElement.VideoAspectRatio:
						{
							var width = (int)formatCheck[Core.FormatChecker.FormatCheckElement.VideoWidth].Item2;
							var height = (int)formatCheck[Core.FormatChecker.FormatCheckElement.VideoHeight].Item2;
							var d = Gcd(width, height);
							builder.AppendLine(string.Format(Properties.Resources.AspectRatioDoesNotFallWithinTheRange + "\t ({0}:{1})", width / d, height / d));
						}
						break;

					case Core.FormatChecker.FormatCheckElement.VideoFramerate:
						{
							var maxFramerate = Defines.TwitterLimitation.MaxFramerate;
							var framerate = (double)item.Value.Item2;
							builder.AppendLine(string.Format(Properties.Resources.FramerateExceeded + "\t ({1:0.00} fps)", maxFramerate, framerate));
						}
						break;

					case Core.FormatChecker.FormatCheckElement.VideoPixelAspectRatio:
						builder.AppendLine(Properties.Resources.PixelAspectRatioIsNotSquare);
						break;

					case Core.FormatChecker.FormatCheckElement.VideoInterlaceMode:
						builder.AppendLine(Properties.Resources.VideoIsNotProgressive);
						break;

					case Core.FormatChecker.FormatCheckElement.AudioCodec:
						builder.AppendLine(Properties.Resources.AudioCodecIsNotAacLc);
						break;

					case Core.FormatChecker.FormatCheckElement.AudioAacObjectType:
						builder.AppendLine(Properties.Resources.AudioCodecIsNotAacLc);
						break;

					case Core.FormatChecker.FormatCheckElement.AudioChannel:
						builder.AppendLine(Properties.Resources.AudioChannelIsNot1Or2);
						break;
				}
			}
			if (builder.Length >= 2) builder.Remove(builder.Length - 2, 2);
			return builder.ToString();
		}

		private static int Gcd(int a, int b)
		{
			if (a < b) return Gcd(b, a);
			int d = 0;
			do
			{
				d = a % b;
				a = b;
				b = d;
			} while (d != 0);
			return a;
		}

		private void OpenClick(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.DefaultExt = "mp4";
			dialog.Filter = "Video files (*.mp4)|*.mp4";

			var result = dialog.ShowDialog(this);
			if (result == true)
			{
				this.FilenameTextBox.Text = dialog.FileName;
			}
		}

		private async void UploadButtonClick(object sender, RoutedEventArgs e)
		{
			this.IsBusy = true;

			try
			{
				var status = await Ctx.UploadAsync(this.StatusTextBox.Text, this.FilenameTextBox.Text);
				this.StatusTextBox.Text = string.Empty;
			}
			catch (TwitterException ex)
			{
				MessageBox.Show(ex.Message, "Error");
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show(ex.Message, "Error");
			}
			finally
			{
				this.IsBusy = false;
			}
		}

		private async void AuthorizeClick(object sender, RoutedEventArgs e)
		{
			var session = await Ctx.AuthorizeAsync();
			Process.Start(session.AuthorizeUri.ToString());

			this.SessionData = new AuthorizationSessionData { Session = session };
			var pinWindow = new PinWindow()
			{
				Owner = this,
				SessionData = this.SessionData,
			};
			pinWindow.Show();
		}

		public async void AuthorizeCallback()
		{
			if (this.SessionData.PinCode != 0)
			{
				await Ctx.SetTokensFromPinCodeAsync(this.SessionData.Session, this.SessionData.PinCode);
				this.SessionData = null;

				this.ScreenNameTextBlock.Text = Ctx.Tokens.ScreenName;
				this.IsSignIn = true;
			}
		}

		protected override async void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			await Ctx.LoadSettingsAsync();

			if (Ctx.IsTokensLoaded)
			{
				this.ScreenNameTextBlock.Text = Ctx.Tokens.ScreenName;
				this.IsSignIn = true;
			}
		}

		protected override async void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			await Ctx.SaveSettingsAsync();
		}


		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
		{
			Interlocked.CompareExchange(ref this.PropertyChanged, null, null)?.Invoke(this, propertyChangedEventArgs);
		}
		private static readonly PropertyChangedEventArgs IsSignInPropertyChangedEventArgs = new PropertyChangedEventArgs("IsSignIn");
		private static readonly PropertyChangedEventArgs IsNotSignInPropertyChangedEventArgs = new PropertyChangedEventArgs("IsNotSignIn");
		private static readonly PropertyChangedEventArgs IsInvalidFilePropertyChangedEventArgs = new PropertyChangedEventArgs("IsInvalidFile");
		private static readonly PropertyChangedEventArgs IsUploadEnabledPropertyChangedEventArgs = new PropertyChangedEventArgs("IsUploadEnabled");
		private static readonly PropertyChangedEventArgs ErrorMessagePropertyChangedEventArgs = new PropertyChangedEventArgs("ErrorMessage");
	}
}