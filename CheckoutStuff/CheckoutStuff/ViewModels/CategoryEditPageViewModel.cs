using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using CheckoutStuff.Configuration;
using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Media.Imaging;

namespace CheckoutStuff.ViewModels;

internal partial class CategoryEditPageViewModel : ObservableObject {
	[ObservableProperty]
	private ObservableCollection<ProductGroup> groups = [];

	[ObservableProperty]
	private ProductGroup? selectedGroup;

	public async Task Load() {
		Groups = new ObservableCollection<ProductGroup>(await Configuration.Configuration.ParseProductInfo());
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
	public void Delete() {
		Groups.Remove(SelectedGroup);
		SelectedGroup = null;
	}

	[RelayCommand]
	public void Add() {
		ProductGroup newGroup = new() {
			ImageName = "",
			Name = "",
			Products = [],
			Image = new BitmapImage()
		};

		Groups.Add(newGroup);
		SelectedGroup = newGroup;
	}
}