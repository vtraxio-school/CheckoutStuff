using CheckoutStuff.Configuration;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.Messaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CategorySelectionPage : Page {
	private ObservableCollection<ProductGroup> productGroups = new(Configuration.Configuration.ParseProductInfo());

	public CategorySelectionPage() {
		InitializeComponent();
	}

	private void CategoryItemView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args) {
		if (args.InvokedItem is ProductGroup group) {
			Frame.Navigate(typeof(ProductSelectionPage), group);
		}
	}
}