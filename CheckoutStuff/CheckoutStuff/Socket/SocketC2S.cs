using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using CheckoutStuff.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SocketIO.Serializer.NewtonsoftJson;
using SocketIO.Serializer.SystemTextJson;

namespace CheckoutStuff.Socket {
	internal class SocketC2S {
		private static SocketC2S? _instance;

		public static SocketC2S GetInstance() {
			_instance ??= new SocketC2S();

			return _instance;
		}

		private readonly SocketIOClient.SocketIO _client;
		public readonly  Guid                    MachineIdentifier = Guid.NewGuid();

		public SocketC2S() {
			_client = new SocketIOClient.SocketIO("http://localhost:3000");
			_client.Serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {
					NamingStrategy = new CamelCaseNamingStrategy()
				}
			});

			Task.Run(async () => {
				await _client.ConnectAsync();

				_client.On("error", response => {
					var error = response.GetValue<string>();
					Debug.WriteLine($"Socket Error: {error}");
				});

				_client.On("couponApplied", response => {
					var result = response.GetValue<CouponAppliedMessageDetails>();
					App.UIDispatcher.TryEnqueue(() => { WeakReferenceMessenger.Default.Send(new CouponAppliedS2CMessage(result)); });
				});

				await _client.EmitAsync("register", response => {
					var result = response.GetValue<string>();
					Debug.WriteLine($"Register: {result}");
				}, new { identifier = MachineIdentifier.ToString() });
			});
		}
	}
}