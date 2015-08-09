using Microsoft.Win32;
using Mntone.TwitterVideoUploader.Core;
using Mntone.Windows.PerMonitorDpiSupport;
using System;
using System.ComponentModel;
using System.Diagnostics;
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

		private bool IsValidPath
		{
			get { return this._IsValidPath; }
			set
			{
				if (this._IsValidPath == value) return;

				this._IsValidPath = value;
				this.RaisePropertyChanged(IsUploadEnabledPropertyChangedEventArgs);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool _IsValidPath = false;

		public bool IsUploadEnabled => this.IsSignIn && !this.IsBusy && this.IsValidPath;

		private CancellationTokenSource LazyEvaluationCancellationTokenSource { get; set; }


		public MainWindow()
		{
			this.InitializeComponent();
		}

		private async void FilenameTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			this.IsValidPath = false;

			if (this.LazyEvaluationCancellationTokenSource != null)
			{
				this.LazyEvaluationCancellationTokenSource.Dispose();
			}

			var filename = this.FilenameTextBox.Text;
			this.LazyEvaluationCancellationTokenSource = new CancellationTokenSource();
			this.IsValidPath = await Task.Factory.StartNew(() => AppContext.IsValidPath(filename), this.LazyEvaluationCancellationTokenSource.Token);
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
		private static readonly PropertyChangedEventArgs IsUploadEnabledPropertyChangedEventArgs = new PropertyChangedEventArgs("IsUploadEnabled");
	}
}