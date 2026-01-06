using System.Text.RegularExpressions;
using CheckoutNiceMobile.Socket;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace CheckoutNiceMobile;

[QueryProperty(nameof(ServerAddress), "serverAddress")]
public partial class CodeScanPage : ContentPage {
	public string ServerAddress { get; set; }
	private bool _Processing { get; set; }

	private SocketIOClient.SocketIO Client;

	public CodeScanPage() {
		InitializeComponent();
		barcodeView.Options = new BarcodeReaderOptions {
			Formats = BarcodeFormats.All,
			AutoRotate = true,
			Multiple = true
		};
	}

	protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e) {
		foreach (var barcode in e.Results)
			Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

		var first = e.Results?.FirstOrDefault();
		if (first is not null) {
			Dispatcher.Dispatch(() => { TrySubmitCoupon(first.Value); });
		}
	}

	void SwitchCameraButton_Clicked(object sender, EventArgs e) {
		barcodeView.CameraLocation = barcodeView.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
	}

	async void SelectCameraButton_Clicked(object sender, EventArgs e) {
		// Get available cameras
		var cameras = await barcodeView.GetAvailableCameras();

		if (cameras.Count == 0) {
			await DisplayAlertAsync("Brak Kamer", "To urządzenie nie ma żadnych kamer.", "OK");
			return;
		}

		// Create a list of camera names for the action sheet
		var cameraNames = cameras.Select(c => c.Name).ToArray();

		// Show action sheet to select camera
		var selectedName = await DisplayActionSheetAsync("Wybierz kamerę", "Anuluj", null, cameraNames);

		if (selectedName != null && selectedName != "Anuluj") {
			// Find the selected camera
			var selectedCamera = cameras.FirstOrDefault(c => c.Name == selectedName);
			if (selectedCamera != null) {
				barcodeView.SelectedCamera = selectedCamera;
				StatusLabel.Text = $"Wybrano: {selectedCamera.Name}";
			}
		}
	}

	void TorchButton_Clicked(object sender, EventArgs e) {
		barcodeView.IsTorchOn = !barcodeView.IsTorchOn;
	}

	private async void ManualButton_Clicked(object? sender, EventArgs e) {
		var result = await DisplayPromptAsync("Wpisywanie ręczne", "Kod kasy:", placeholder: "00000000-0000-0000-0000-000000000000");

		TrySubmitCoupon(result);
	}

	void TrySubmitCoupon(string identifier) {
		if (!Guid.TryParse(identifier, out var guid)) {
			StatusLabel.Text = "Niepoprawny kod";
			return;
		}

		barcodeView.IsDetecting = false;

		StatusLabel.Text = $"Łączanie z: {ServerAddress}";

		Client = new SocketIOClient.SocketIO(ServerAddress);

		Task.Run(async () => {
			Client.On("error", res => {
				Dispatcher.Dispatch(async () => {
					StatusLabel.Text = res.GetValue<SocketError>().message;
					await Client.DisconnectAsync();
					barcodeView.IsDetecting = true;
				});
			});
			await Client.ConnectAsync();
			await Client.EmitAsync("applyCoupon", res => {
				Dispatcher.Dispatch(async () => {
					var result = res.GetValue<string>();
					StatusLabel.Text = result;
					await Client.DisconnectAsync();
					barcodeView.IsDetecting = true;

					if (result == "OK") {
						await Shell.Current.GoToAsync("..");
					}
				});
			}, new { identifier, discountPercentage = 0.3f });
			Dispatcher.Dispatch(() => { StatusLabel.Text = "..."; });
		});
	}
}