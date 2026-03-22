using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TemGen;

public sealed class GeneratedOutputManifest
{
	private static readonly JsonSerializerOptions SerializerOptions = new()
	{
		WriteIndented = true
	};

	public const string FileName = ".temgen-manifest.json";

	public HashSet<string> Files { get; init; } = new(StringComparer.Ordinal);

	public static string GetManifestPath(string outputPath)
	{
		return Path.Combine(outputPath, FileName);
	}

	public static string NormalizeRelativePath(string relativePath)
	{
		return relativePath.Replace('\\', '/');
	}

	public static string GetAbsolutePath(string outputPath, string relativePath)
	{
		return Path.Combine(outputPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
	}

	public static async Task<GeneratedOutputManifest> LoadAsync(string outputPath, CancellationToken cancellationToken = default)
	{
		var manifestPath = GetManifestPath(outputPath);

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
					files.Add(NormalizeRelativePath(file.GetString()));
				}
			}
		}
		else if (filesElement.ValueKind == JsonValueKind.Object)
		{
			foreach (var property in filesElement.EnumerateObject())
			{
				files.Add(NormalizeRelativePath(property.Name));
			}
		}

		return new GeneratedOutputManifest
		{
			Files = files
		};
	}

	public async Task SaveAsync(string outputPath, CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(outputPath);

		var manifestPath = GetManifestPath(outputPath);
		await using var stream = File.Create(manifestPath);
		await JsonSerializer.SerializeAsync(stream, this, SerializerOptions, cancellationToken).ConfigureAwait(false);
	}

}

public sealed class GeneratedOutputCleanupResult
{
	public int DeletedCount { get; init; }
}

public static class GeneratedOutputCleaner
{
	public static async Task<GeneratedOutputCleanupResult> CleanupAsync(
		string outputPath,
		GeneratedOutputManifest previousManifest,
		GeneratedOutputManifest currentManifest,
		CancellationToken cancellationToken = default)
	{
		if (previousManifest == null)
		{
			return new GeneratedOutputCleanupResult();
		}

		var currentFiles = new HashSet<string>(currentManifest.Files, StringComparer.Ordinal);
		var deletedCount = 0;

		foreach (var previousFile in previousManifest.Files)
		{
			if (currentFiles.Contains(previousFile))
			{
				continue;
			}

			var absolutePath = GeneratedOutputManifest.GetAbsolutePath(outputPath, previousFile);

			if (!File.Exists(absolutePath))
			{
				continue;
			}

			File.Delete(absolutePath);
			deletedCount++;
			DeleteEmptyDirectories(outputPath, Path.GetDirectoryName(absolutePath));
		}

		return new GeneratedOutputCleanupResult
		{
			DeletedCount = deletedCount,
		};
	}

	private static void DeleteEmptyDirectories(string outputPath, string directoryPath)
	{
		while (!string.IsNullOrWhiteSpace(directoryPath)
			&& !Path.GetFullPath(directoryPath).TrimEnd(Path.DirectorySeparatorChar).Equals(Path.GetFullPath(outputPath).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)
			&& Directory.Exists(directoryPath)
			&& !Directory.EnumerateFileSystemEntries(directoryPath).Any())
		{
			Directory.Delete(directoryPath);
			directoryPath = Path.GetDirectoryName(directoryPath);
		}
	}
}