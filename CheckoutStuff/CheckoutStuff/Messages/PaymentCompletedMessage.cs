using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CheckoutStuff.Messages;

internal enum PaymentType {
	Card,
	Cash
}

internal class PaymentCompletedMessage(PaymentType value) : ValueChangedMessage<PaymentType>(value);