using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using CheckoutStuff.Configuration;
using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace CheckoutStuff.ViewModels;

internal partial class ProductEditPageViewModel(MenuFlyout menu) : ObservableObject {
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Products))]
	private ObservableCollection<ProductGroup> groups = [];

	[ObservableProperty]
	private ObservableCollection<Product> products = [];

	private MenuFlyout menu = menu;

	[ObservableProperty]
	private Product? selectedProduct;

	public async Task Load() {
		Groups = new ObservableCollection<ProductGroup>(await Configuration.Configuration.ParseProductInfo());
		Products = new ObservableCollection<Product>(Groups.SelectMany(g => g.Products));
		menu.Items.Clear();
		foreach (var group in Groups) {
			menu.Items.Add(new MenuFlyoutItem {
				Text = group.Name,
				Command = AddCommand,
				CommandParameter = group
			});
		}
	}

	[RelayCommand]
	public async Task Save() {
		var local = ApplicationData.Current.LocalFolder;
		var cfgFolder = await local.CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists);
		var configFile = await cfgFolder.GetFileAsync("config.rcf");

		var config = "SECTION PROD\n";

		foreach (var productGroup in groups) {
			config += $"GROUP \"{productGroup.Name}\" {productGroup.ImageName}\n";
			foreach (var product in productGroup.Products) {
				var priceClass = product.PriceClass == PriceClass.Single ? "SZT" : "KG";
				config += $"\tPRODUCT \"{product.Name}\" {product.ImageName} {priceClass} {product.Price:F}\n";
			}

			config += $"ENDGROUP\n";
		}

		await FileIO.WriteTextAsync(configFile, config);
		await Load();
		WeakReferenceMessenger.Default.Send(new ReloadProductListsMessage());
	}

	[RelayCommand]
	public void Add(ProductGroup group) {
		Product product = new() {
			ImageName = "",
			Image = new BitmapImage(),
			Name = "",
			Price = 0,
			PriceClass = PriceClass.Single,
		};

		group.Products.Add(product);
		Products = new ObservableCollection<Product>(Groups.SelectMany(g => g.Products));
		selectedProduct = product;
	}

	[RelayCommand]
	public void Delete() {
		foreach (var group in Groups) {
			group.Products.Remove(SelectedProduct);
		}

		OnPropertyChanged(nameof(Groups));
		Products = new ObservableCollection<Product>(Groups.SelectMany(g => g.Products));

		SelectedProduct = null;
	}
}