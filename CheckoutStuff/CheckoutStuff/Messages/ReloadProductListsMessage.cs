using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CheckoutStuff.Messages;

internal class ReloadProductListsMessage() : ValueChangedMessage<object?>(null);