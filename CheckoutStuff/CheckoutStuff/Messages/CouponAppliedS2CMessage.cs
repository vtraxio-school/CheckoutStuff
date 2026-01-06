using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CheckoutStuff.Messages;

internal class CouponAppliedMessageDetails {
	public double discountPercentage;
}

internal class CouponAppliedS2CMessage(CouponAppliedMessageDetails details) : ValueChangedMessage<CouponAppliedMessageDetails>(details);