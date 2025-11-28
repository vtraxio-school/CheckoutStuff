using System;
using System.Collections.Generic;
using CheckoutStuff.Configuration;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CategorySelectionPage : Page, INotifyPropertyChanged {
	private ObservableCollection<ProductGroup> productGroups = [];

	public CategorySelectionPage() {
		InitializeComponent();
		Loading += async (sender, args) => {
			productGroups = new ObservableCollection<ProductGroup>(await Configuration.Configuration.ParseProductInfo());
			OnPropertyChanged(nameof(productGroups));
		};
		WeakReferenceMessenger.Default.Register<ReloadProductListsMessage>(this, async (r, m) => {
			productGroups = new ObservableCollection<ProductGroup>(await Configuration.Configuration.ParseProductInfo());
			OnPropertyChanged(nameof(productGroups));
		});
	}

	private void CategoryItemView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args) {
		if (args.InvokedItem is ProductGroup group) {
			Frame.Navigate(typeof(ProductSelectionPage), group);
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}