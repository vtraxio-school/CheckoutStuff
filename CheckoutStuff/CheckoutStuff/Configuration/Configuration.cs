using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public static string ConfigurationFile = "Assets/Definitions/default.rcf";

	private static List<string> GetSection(string file, string section) {
		var content = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, file));
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

	private static BitmapImage OpenImage(string name) {
		var path = Path.Combine(AppContext.BaseDirectory, "Assets/Images/", name).Replace("/", "\\");
		var file = StorageFile.GetFileFromPathAsync(path).AsTask().Result;
		using IRandomAccessStream fileStream = file.OpenReadAsync().AsTask().Result;

		BitmapImage image = new();
		image.SetSource(fileStream);

		return image;
	}

	public static List<ProductGroup> ParseProductInfo() {
		var productInfo = GetSection(ConfigurationFile, "PROD");
		var commands = productInfo.Select(SplitCommand).ToList();

		var productGroups = new List<ProductGroup>();
		ProductGroup? currentGroup = null;

		foreach (var (cmd, args) in commands) {
			switch (cmd) {
				case "GROUP":
					if (args.Length < 2) {
						throw new Exception("GROUP command requires at least 2 arguments: Name and Image");
					}


					currentGroup = new ProductGroup {
						Name = args[0],
						Image = OpenImage(args[1]),
						Products = []
					};

					break;
				case "ENDGROUP":
					if (currentGroup != null) {
						productGroups.Add(currentGroup);
						currentGroup = null;
					} else {
						throw new Exception("ENDGROUP command found without a corresponding GROUP");
					}

					break;
				case "PRODUCT":
					var product = new Product {
						Name = args[0],
						Image = OpenImage(args[1]),
						PriceClass = args[2] == "KG" ? PriceClass.Weight : PriceClass.Single,
						Price = float.Parse(args[3])
					};
					currentGroup?.Products.Add(product);
					break;
			}
		}

		return productGroups;
	}
}