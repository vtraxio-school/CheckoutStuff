using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CheckoutStuff.Messages;

internal struct CouponAppliedMessageDetails {
	[JsonPropertyName("discountPercentage")]
	public float DiscountPercentage;
}

internal class CouponAppliedS2CMessage(CouponAppliedMessageDetails details) : ValueChangedMessage<CouponAppliedMessageDetails>(details);