using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StartPage : Page {
	public StartPage() {
		InitializeComponent();
	}

	private void StartButton_OnClick(object sender, RoutedEventArgs e) {
		Frame.Navigate(typeof(CategorySelectionPage));
	}
}