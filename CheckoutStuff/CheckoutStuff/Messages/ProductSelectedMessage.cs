using CheckoutStuff.Configuration;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CheckoutStuff.Messages;

internal class ProductSelectedMessage(Product value) : ValueChangedMessage<Product>(value);