using System.Collections.ObjectModel;
using System.Diagnostics;
using CheckoutStuff.Configuration;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace CheckoutStuff.ViewModels;

internal partial class ScanningPageViewModel : ObservableObject {
	[ObservableProperty]
	private ObservableCollection<ProductGroup> productGroups = new(Configuration.Configuration.ParseProductInfo());

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(GroupsVisibility))]
	[NotifyPropertyChangedFor(nameof(ProductVisibility))]
	private ProductGroup? selectedGroup;

	public Visibility GroupsVisibility => SelectedGroup is null ? Visibility.Visible : Visibility.Collapsed;
	public Visibility ProductVisibility => SelectedGroup is null ? Visibility.Collapsed : Visibility.Visible;

	[RelayCommand]
	public void SelectGroup(ProductGroup? group) {
		SelectedGroup = group;
		Debug.WriteLine($"{GroupsVisibility}  {SelectedGroup}");
	}
}