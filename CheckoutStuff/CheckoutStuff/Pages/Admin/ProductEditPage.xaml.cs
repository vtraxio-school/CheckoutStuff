using System;
using System.Diagnostics;
using Windows.Globalization.NumberFormatting;
using Windows.Storage;
using CheckoutStuff.Configuration;
using CheckoutStuff.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages.Admin;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProductEditPage : Page {
	private ProductEditPageViewModel viewModel;

	public ProductEditPage() {
		InitializeComponent();
		viewModel = new(menuFlyout);
		Loading += async (sender, args) => { await viewModel.Load(); };

		IncrementNumberRounder rounder = new() {
			Increment = 0.01,
			RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
		};
		DecimalFormatter formatter = new() {
			IntegerDigits = 1,
			FractionDigits = 2,
			NumberRounder = rounder
		};
		FormattedNumberBox.NumberFormatter = formatter;
	}

	private void ProductList_OnItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args) {
		viewModel.SelectedProduct = args.InvokedItem as Product;
	}

	private async void OpenFolderButton_OnClick(object sender, RoutedEventArgs e) {
		var local = ApplicationData.Current.LocalFolder;
		var cfgFolder = await local.CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists);

		Process.Start("explorer.exe", cfgFolder.Path);
	}
}