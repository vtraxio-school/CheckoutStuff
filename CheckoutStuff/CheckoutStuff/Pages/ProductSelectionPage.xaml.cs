using CheckoutStuff.Configuration;
using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProductSelectionPage : Page {
	private ProductGroup? _category;

	public ProductSelectionPage() {
		InitializeComponent();
	}

	private void ProductBackButton_OnClicked(object sender, RoutedEventArgs e) {
		Frame.GoBack();
	}

	private void ProductItemView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args) {
		if (args.InvokedItem is Product product) {
			WeakReferenceMessenger.Default.Send(new ProductSelectedMessage(product));
		}
	}

	protected override void OnNavigatedTo(NavigationEventArgs e) {
		base.OnNavigatedTo(e);

		if (e.Parameter is ProductGroup category) {
			_category = category;
		}
	}
}