using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CheckoutStuff;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StaffSignInContentDialog : ContentDialog {
	public bool Authorized { get; private set; }

	public StaffSignInContentDialog() {
		InitializeComponent();
	}

	private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
		if (string.IsNullOrEmpty(userNameTextBox.Text)) {
			errorInfoBar.Message = "Login jest wymagany.";
			errorInfoBar.IsOpen = true;
		} else if (string.IsNullOrEmpty(passwordTextBox.Password)) {
			errorInfoBar.Message = "Hasło jest wymagane.";
			errorInfoBar.IsOpen = true;
		} else if (userNameTextBox.Text != "admin" || passwordTextBox.Password != "qwerty") {
			errorInfoBar.Message = "Dane niepoprawne";
			errorInfoBar.IsOpen = true;
		}

		if (errorInfoBar.IsOpen) {
			args.Cancel = true;
			return;
		}

		Authorized = true;
	}

	private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
		Authorized = false;
	}

	private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
		if (!string.IsNullOrEmpty(userNameTextBox.Text)) {
			errorInfoBar.Message = string.Empty;
			errorInfoBar.IsOpen = false;
		}
	}

	private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e) {
		if (!string.IsNullOrEmpty(passwordTextBox.Password)) {
			errorInfoBar.Message = string.Empty;
			errorInfoBar.IsOpen = false;
		}
	}
}