using System.Windows;
using System.Windows.Controls;

namespace Mntone.TwitterVideoUploader.Views.Controls
{
	public sealed class DropDownMenu : ContextMenu
	{
		static DropDownMenu()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(DropDownMenu),
				new FrameworkPropertyMetadata(typeof(DropDownMenu)));
		}
	}
}
