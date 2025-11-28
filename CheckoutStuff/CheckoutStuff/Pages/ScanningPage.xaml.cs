using System;
using CheckoutStuff.ViewModels;
using CheckoutStuff.Windows;
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

	private void AddedProducts_ItemSelected(ItemsView sender, ItemsViewSelectionChangedEventArgs args) {
		if (AddedItems.SelectedItem is AddedProduct product) {
			viewModel.SelectItemCommand.Execute(product.Product);
		}
	}

	private void PurchaseButton_OnClick(object sender, RoutedEventArgs e) {
		if (viewModel.AddedProducts.Count > 0) {
			InteractiveFrame.Navigate(typeof(PaymentMetodPage));
		}
	}

	private async void StaffLoginButton_OnClick(object sender, RoutedEventArgs e) {
		StaffSignInContentDialog dialog = new() {
			XamlRoot = XamlRoot
		};

		await dialog.ShowAsync();

		if (dialog.Authorized) {
			AdminWindow window = new();
			window.Activate();
		}
	}
}