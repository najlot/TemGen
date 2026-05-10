using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TemGen;

public sealed class GeneratedOutputManifest
{
	private static readonly JsonSerializerOptions _serializerOptions = new()
	{
		WriteIndented = true
	};

	public const string FileName = ".temgen-manifest.json";

	public HashSet<string> Files { get; init; } = new(StringComparer.Ordinal);

	public static async Task<GeneratedOutputManifest> LoadAsync(string outputPath, CancellationToken cancellationToken = default)
	{
		var manifestPath = Path.Combine(outputPath, FileName);

		if (!File.Exists(manifestPath))
		{
			return null;
		}

		await using var stream = File.OpenRead(manifestPath);
		using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

		if (!document.RootElement.TryGetProperty(nameof(Files), out var filesElement))
		{
			return null;
		}

		var files = new HashSet<string>(StringComparer.Ordinal);

		if (filesElement.ValueKind == JsonValueKind.Array)
		{
			foreach (var file in filesElement.EnumerateArray())
			{
				if (file.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(file.GetString()))
				{
					files.Add(file.GetString().Replace('\\', '/'));
				}
			}
		}
		else if (filesElement.ValueKind == JsonValueKind.Object)
		{
			foreach (var property in filesElement.EnumerateObject())
			{
				files.Add(property.Name.Replace('\\', '/'));
			}
		}

		return new GeneratedOutputManifest
		{
			Files = files
		};
	}

	public async Task SaveAsync(string outputPath)
	{
		Directory.CreateDirectory(outputPath);

		var manifestPath = Path.Combine(outputPath, FileName);
		await using var stream = File.Create(manifestPath);
		await JsonSerializer.SerializeAsync(stream, this, _serializerOptions).ConfigureAwait(false);
	}
}
