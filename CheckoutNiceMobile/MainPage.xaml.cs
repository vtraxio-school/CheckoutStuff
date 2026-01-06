using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace CheckoutNiceMobile {
	public partial class MainPage : ContentPage {
		public MainPage() {
			InitializeComponent();
		}

		private async void ScanCode_OnClicked(object? sender, EventArgs e) {
			await Shell.Current.GoToAsync($"scan?serverAddress={ServerAddress.Text}");
		}
	}
}