using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutNiceMobile.Socket;

internal class SocketError {
	public object error { get; set; }
	public string message { get; set; }
}