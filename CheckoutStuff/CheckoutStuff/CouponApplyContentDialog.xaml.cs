using System;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Controls;
using CheckoutStuff.Socket;
using Microsoft.UI.Xaml.Media.Imaging;
using QRCoder;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CouponApplyContentDialog : ContentDialog {
	private string      _codeText;
	private BitmapImage _qrCodeImage;

	public CouponApplyContentDialog() {
		_codeText = SocketC2S.GetInstance().MachineIdentifier.ToString();
		var qrGenerator = new QRCodeGenerator();
		var qrCodeData = qrGenerator.CreateQrCode(_codeText, QRCodeGenerator.ECCLevel.H);
		var qrCode = new PngByteQRCode(qrCodeData);
		var bytes = qrCode.GetGraphic(20);

		using (var stream = new InMemoryRandomAccessStream()) {
			using (var writer = new DataWriter(stream.GetOutputStreamAt(0))) {
				writer.WriteBytes(bytes);
				_ = writer.StoreAsync().AsTask().Result;
			}

			_qrCodeImage = new BitmapImage();
			_qrCodeImage.SetSource(stream);
		}

		InitializeComponent();
	}
}