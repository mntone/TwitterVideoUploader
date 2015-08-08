using System.Windows;
using Mntone.TwitterVideoUploader.Core;

namespace Mntone.TwitterVideoUploader
{
	public partial class App : Application
	{
		public AppContext Context { get; } = new AppContext();
	}
}
