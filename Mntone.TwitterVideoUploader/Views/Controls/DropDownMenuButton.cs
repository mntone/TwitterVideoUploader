using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Mntone.TwitterVideoUploader.Views.Controls
{
	public sealed class DropDownMenuButton : ToggleButton
	{
		static DropDownMenuButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(DropDownMenuButton),
				new FrameworkPropertyMetadata(typeof(DropDownMenuButton)));
		}

		public DropDownMenuButton()
		{
			var binding = new Binding("DropDownMenu.IsOpen") { Source = this };
			this.SetBinding(IsCheckedProperty, binding);
		}

		public static readonly DependencyProperty DropDownMenuProperty
			= DependencyProperty.Register(
				"DropDownMenu",
				typeof(DropDownMenu),
				typeof(DropDownMenuButton),
				new UIPropertyMetadata(null, OnDropDownMenuChanged));

		public DropDownMenu DropDownMenu
		{
			get { return (DropDownMenu)this.GetValue(DropDownMenuProperty); }
			set { this.SetValue(DropDownMenuProperty, value); }
		}

		private static void OnDropDownMenuChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var button = (DropDownMenuButton)o;
			var contextMenu = (DropDownMenu)e.NewValue;
			if (contextMenu == null) return;

			contextMenu.Placement = PlacementMode.Bottom;
			contextMenu.PlacementTarget = button;
			contextMenu.StaysOpen = false;
		}
	}
}
