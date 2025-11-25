using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Networking.NetworkOperators;
using CheckoutStuff.Configuration;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace CheckoutStuff.ViewModels;

internal partial class AddedProduct : ObservableObject {
	public Product Product;

	[ObservableProperty]
	private int count;

	public string StrCountingPrice => $"{Count} x {Product.Price:C}";

	public string StrTotalPrice => $"{Product.Price * Count:C}";
}

internal partial class ScanningPageViewModel : ObservableObject {
	[ObservableProperty]
	private ObservableCollection<ProductGroup> productGroups = new(Configuration.Configuration.ParseProductInfo());

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(GroupsVisibility))]
	[NotifyPropertyChangedFor(nameof(ProductVisibility))]
	private ProductGroup? selectedGroup;

	[ObservableProperty]
	private ObservableCollection<AddedProduct> addedProducts = [];

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(DeleteItemCommand))]
	private Product? selectedProduct;

	public string StrTotal => AddedProducts.Select(x => x.Count * x.Product.Price).Sum().ToString("C");

	public Visibility GroupsVisibility => SelectedGroup is null ? Visibility.Visible : Visibility.Collapsed;
	public Visibility ProductVisibility => SelectedGroup is null ? Visibility.Collapsed : Visibility.Visible;

	[RelayCommand]
	public void SelectGroup(ProductGroup? group) {
		SelectedGroup = group;
	}

	[RelayCommand]
	public void AddProduct(Product product) {
		if (AddedProducts.Any(x => x.Product == product)) {
			var item = AddedProducts.First(x => x.Product == product);
			var index = AddedProducts.IndexOf(item);
			AddedProducts.Remove(item);
			item.Count++;
			AddedProducts.Insert(index, item);
		} else {
			AddedProducts.Add(new AddedProduct {
				Count = 1,
				Product = product
			});
		}

		OnPropertyChanged(nameof(StrTotal));
	}

	[RelayCommand]
	public void SelectItem(Product product) {
		SelectedProduct = product;
	}

	[RelayCommand(CanExecute = nameof(CanDelete))]
	public void DeleteItem() {
		var item = AddedProducts.First(x => x.Product == SelectedProduct);
		var index = AddedProducts.IndexOf(item);
		AddedProducts.Remove(item);
		item.Count--;

		if (item.Count > 0) {
			AddedProducts.Insert(index, item);
		}

		OnPropertyChanged(nameof(StrTotal));
		SelectedProduct = null;
	}

	bool CanDelete() {
		return SelectedProduct is not null;
	}
}