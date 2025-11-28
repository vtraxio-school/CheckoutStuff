using CheckoutStuff.Configuration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using Windows.Storage;
using CheckoutStuff.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages.Admin;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CategoryEditPage : Page {
	private CategoryEditPageViewModel viewModel = new();

	public CategoryEditPage() {
		InitializeComponent();
		Loading += async (sender, args) => { await viewModel.Load(); };
	}

	private void CategoryList_OnItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args) {
		viewModel.SelectedGroup = args.InvokedItem as ProductGroup;
	}

	private async void OpenFolderButton_OnClick(object sender, RoutedEventArgs e) {
		var local = ApplicationData.Current.LocalFolder;
		var cfgFolder = await local.CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists);

		Process.Start("explorer.exe", cfgFolder.Path);
	}
}