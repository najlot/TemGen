using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TemGen;

public static class GeneratedOutputCleaner
{
	public static int Cleanup(
		string outputPath,
		GeneratedOutputManifest previousManifest,
		GeneratedOutputManifest currentManifest)
	{
		if (previousManifest.Files.Count == 0)
		{
			return 0;
		}

		var currentFiles = new HashSet<string>(currentManifest.Files, StringComparer.Ordinal);
		var deletedCount = 0;
		var directoriesToCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var normalizedOutputPath = Path.GetFullPath(outputPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

		foreach (var previousFile in previousManifest.Files)
		{
			if (currentFiles.Contains(previousFile))
			{
				continue;
			}

			var absolutePath = Path.Combine(outputPath, previousFile.Replace('/', Path.DirectorySeparatorChar));

			try
			{
				if (File.Exists(absolutePath))
				{
					File.Delete(absolutePath);
					deletedCount++;

					var dirName = Path.GetDirectoryName(absolutePath);
					if (!string.IsNullOrWhiteSpace(dirName))
					{
						directoriesToCheck.Add(dirName);
					}
				}
			}
			catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
			{
				Console.WriteLine($"Could not delete file: {absolutePath}. {ex}");
			}
		}

		var sortedDirectories = directoriesToCheck
			.OrderByDescending(d => d.Length)
			.ToList();

		foreach (var dir in sortedDirectories)
		{
			DeleteEmptyDirectories(normalizedOutputPath, dir);
		}

		return deletedCount;
	}

	private static void DeleteEmptyDirectories(string normalizedOutputPath, string directoryPath)
	{
		var currentDir = Path.GetFullPath(directoryPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

		while (!string.IsNullOrWhiteSpace(currentDir))
		{
			if (currentDir.Equals(normalizedOutputPath, StringComparison.OrdinalIgnoreCase))
			{
				break;
			}

			try
			{
				if (Directory.Exists(currentDir) && !Directory.EnumerateFileSystemEntries(currentDir).Any())
				{
					Directory.Delete(currentDir);
					currentDir = Path.GetDirectoryName(currentDir)?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				}
				else
				{
					break;
				}
			}
			catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
			{
				break;
			}
		}
	}
}