using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PaymentMetodPage : Page {
	public PaymentMetodPage() {
		InitializeComponent();
	}

	private void BackButton_OnClick(object sender, RoutedEventArgs e) {
		Frame.Navigate(typeof(CategorySelectionPage));
	}

	private void CardPayButton_Click(object sender, RoutedEventArgs e) {
		WeakReferenceMessenger.Default.Send(new PaymentCompletedMessage(PaymentType.Card));
		PaymentCompleted();
	}

	private void CashPayButton_Click(object sender, RoutedEventArgs e) {
		WeakReferenceMessenger.Default.Send(new PaymentCompletedMessage(PaymentType.Cash));
		PaymentCompleted();
	}

	private void PaymentCompleted() {
		Frame.Navigate(typeof(StartPage));
	}
}