using CheckoutStuff.Pages.Admin;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AdminWindow : Window {
	public AdminWindow() {
		InitializeComponent();
	}

	private void TabView_Loaded(object sender, RoutedEventArgs e) {
		var view = sender as TabView;
		var categoriesFrame = new Frame();
		categoriesFrame.Navigate(typeof(CategoryEditPage));
		TabViewItem categories = new() {
			Header = "Kategorie",
			Content = categoriesFrame
		};
		var productsFrame = new Frame();
		productsFrame.Navigate(typeof(ProductEditPage));
		TabViewItem products = new() {
			Header = "Produkty",
			Content = productsFrame
		};
		view.TabItems.Add(categories);
		view.TabItems.Add(products);
	}
}