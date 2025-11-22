using CheckoutStuff.Configuration;
using CheckoutStuff.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ScanningPage : Page {
	private ScanningPageViewModel viewModel;

	public ScanningPage() {
		InitializeComponent();
		viewModel = new ScanningPageViewModel();
	}

	private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
		Configuration.Configuration.ParseProductInfo();
	}

	private void GroupItemView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args) {
		var group = args.InvokedItem as ProductGroup;
		viewModel.SelectGroupCommand.Execute(group);
	}

	private void CategoryBackButton_OnClicked(object sender, RoutedEventArgs e) {
		viewModel.SelectGroupCommand.Execute(null);
	}
}