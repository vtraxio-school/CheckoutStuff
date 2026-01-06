namespace CheckoutNiceMobile {
	public partial class AppShell : Shell {
		public AppShell() {
			InitializeComponent();

			Routing.RegisterRoute("scan", typeof(CodeScanPage));
		}
	}
}