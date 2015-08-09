using System;
using System.Windows.Controls;
using System.Linq;
using Mntone.Windows.PerMonitorDpiSupport;

namespace Mntone.TwitterVideoUploader.Views
{
	public partial class PinWindow : PerMonitorDpiSupportWindow
	{
		public AuthorizationSessionData SessionData { get; set; }

		public PinWindow()
		{
			this.InitializeComponent();
		}

		private void PincodeTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			var text = this.PincodeTextBox.Text;
			if (text.Length == 7 && text.All(c => c >= '0' && c <= '9'))
			{
				this.PostProcess(Convert.ToUInt32(text));
			}
		}

		private void PostProcess(uint pinCode)
		{
			if (this.SessionData == null)
			{
				throw new InvalidOperationException("SessionData is null");
			}
			this.SessionData.PinCode = pinCode;
			this.Close();
			(this.Owner as MainWindow)?.AuthorizeCallback();
		}
	}
}