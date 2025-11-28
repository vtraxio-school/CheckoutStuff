using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Networking.NetworkOperators;
using CheckoutStuff.Configuration;
using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
	[NotifyPropertyChangedFor(nameof(CanCheckout))]
	private ObservableCollection<AddedProduct> addedProducts = [];

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(DeleteItemCommand))]
	private Product? selectedProduct;

	public string StrTotal => AddedProducts.Select(x => x.Count * x.Product.Price).Sum().ToString("C");
	public bool CanCheckout => AddedProducts.Count > 0;

	public ScanningPageViewModel() {
		WeakReferenceMessenger.Default.Register<ProductSelectedMessage>(this, (r, m) => { AddProduct(m.Value); });
		WeakReferenceMessenger.Default.Register<PaymentCompletedMessage>(this, (r, m) => {
			var paragon = "Fajny sklep\n================================\n";

			foreach (var product in AddedProducts) {
				paragon += $"{product.Product.Name} - {product.StrCountingPrice} - {product.StrTotalPrice}\n";
			}

			paragon += $"================================\n{StrTotal} ";
			paragon += m.Value == PaymentType.Card ? "Karta" : "Gotówka";

			var name = Path.Join(AppContext.BaseDirectory, $"Paragon_{DateTime.Now.ToBinary()}.txt");
			File.WriteAllText(name, paragon);
			Process.Start("notepad.exe", name);

			AddedProducts.Clear();
			AddedItemsChanged();
		});
	}

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

		AddedItemsChanged();
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

		AddedItemsChanged();
		SelectedProduct = null;
	}

	void AddedItemsChanged() {
		OnPropertyChanged(nameof(StrTotal));
		OnPropertyChanged(nameof(CanCheckout));
	}

	bool CanDelete() {
		return SelectedProduct is not null;
	}
}