using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;

namespace CheckoutStuff.Configuration;

internal enum PriceClass {
	Single,
	Weight
}

internal class Product {
	public required string Name { get; set; }
	public required BitmapImage Image { get; set; }
	public PriceClass PriceClass { get; set; }
	public float Price { get; set; }
	public string StrPrice => $"{Price:C}";

	public string StrPriceClass {
		get {
			return PriceClass switch {
				PriceClass.Weight => " / KG",
				_                 => ""
			};
		}
	}
}

internal class ProductGroup {
	public required string Name { get; set; }
	public required BitmapImage Image { get; set; }
	public required List<Product> Products { get; set; }
}

internal static class Configuration {
	private static async Task<List<string>> GetSection(StorageFile file, string section) {
		var content = await FileIO.ReadLinesAsync(file);
		content = content.Select(x => x.Trim()).ToArray();

		var inSection = new List<string>();
		var duringSection = false;
		foreach (var line in content) {
			if (line == $"SECTION {section}") {
				duringSection = true;
				continue;
			}

			if (line.StartsWith("SECTION ") && duringSection) {
				break;
			}

			if (string.IsNullOrWhiteSpace(line.Trim())) {
				continue;
			}

			if (duringSection) {
				inSection.Add(line);
			}
		}

		return inSection;
	}

	private static (string, string[]) SplitCommand(string command) {
		var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var cmd = parts[0];
		var stringArgs = string.Join(' ', parts.Skip(1));

		var args = new List<string>();
		var currentArg = "";
		var inQuotes = false;
		foreach (var ch in stringArgs) {
			if (ch == '"') {
				inQuotes = !inQuotes;
				continue;
			}

			if (ch == ' ' && !inQuotes) {
				if (string.IsNullOrWhiteSpace(currentArg)) continue;
				args.Add(currentArg);
				currentArg = "";
			} else {
				currentArg += ch;
			}
		}

		if (!string.IsNullOrWhiteSpace(currentArg)) {
			args.Add(currentArg);
		}

		return (cmd, args.ToArray());
	}

	private static async Task<BitmapImage> OpenImage(StorageFolder folder, string name) {
		var file = await folder.GetFileAsync(name);
		using var fileStream = await file.OpenReadAsync();

		BitmapImage image = new();
		await image.SetSourceAsync(fileStream);

		return image;
	}

	public static async Task<List<ProductGroup>> ParseProductInfo() {
		var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists);
		try {
			var f = await folder.GetFileAsync("config.rcf");
		} catch {
			System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(AppContext.BaseDirectory, "Assets/DefaultStore.zip"), folder.Path);
		}

		var config = await folder.GetFileAsync("config.rcf");

		var productInfo = await GetSection(config, "PROD");
		var commands = productInfo.Select(SplitCommand).ToList();

		var productGroups = new List<ProductGroup>();
		ProductGroup? currentGroup = null;

		foreach (var (cmd, args) in commands) {
			switch (cmd) {
				case "GROUP": {
					if (args.Length < 2) {
						throw new Exception("GROUP command requires at least 2 arguments: Name and Image");
					}

					var image = await OpenImage(folder, args[1]);
					currentGroup = new ProductGroup {
						Name = args[0],
						Image = image,
						Products = []
					};
				}
					break;
				case "ENDGROUP":
					if (currentGroup != null) {
						productGroups.Add(currentGroup);
						currentGroup = null;
					} else {
						throw new Exception("ENDGROUP command found without a corresponding GROUP");
					}

					break;
				case "PRODUCT": {
					var image = await OpenImage(folder, args[1]);
					var product = new Product {
						Name = args[0],
						Image = image,
						PriceClass = args[2] == "KG" ? PriceClass.Weight : PriceClass.Single,
						Price = float.Parse(args[3])
					};
					currentGroup?.Products.Add(product);
				}
					break;
			}
		}

		return productGroups;
	}
}