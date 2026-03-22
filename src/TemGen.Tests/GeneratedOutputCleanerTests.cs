using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TemGen.Tests;

public class GeneratedOutputCleanerTests
{
	[Fact]
	public async Task CleanupAsync_must_delete_unchanged_stale_generated_files()
	{
		var outputPath = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(Path.Combine(outputPath, "Generated"));
		var staleFilePath = Path.Combine(outputPath, "Generated", "Old.txt");
		await File.WriteAllTextAsync(staleFilePath, "old content");

		try
		{
			var previousManifest = new GeneratedOutputManifest
			{
				Files = ["Generated/Old.txt"]
			};

			var currentManifest = new GeneratedOutputManifest();

			var result = await GeneratedOutputCleaner.CleanupAsync(outputPath, previousManifest, currentManifest);

			Assert.Equal(1, result.DeletedCount);
			Assert.False(File.Exists(staleFilePath));
			Assert.False(Directory.Exists(Path.Combine(outputPath, "Generated")));
		}
		finally
		{
			if (Directory.Exists(outputPath))
			{
				Directory.Delete(outputPath, true);
			}
		}
	}

	[Fact]
	public async Task CleanupAsync_must_delete_stale_files_even_if_they_were_modified()
	{
		var outputPath = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(Path.Combine(outputPath, "Generated"));
		var staleFilePath = Path.Combine(outputPath, "Generated", "Old.txt");
		await File.WriteAllTextAsync(staleFilePath, "old content");

		try
		{
			var previousManifest = new GeneratedOutputManifest
			{
				Files = ["Generated/Old.txt"]
			};

			await File.WriteAllTextAsync(staleFilePath, "user changed this file");
			var currentManifest = new GeneratedOutputManifest();

			var result = await GeneratedOutputCleaner.CleanupAsync(outputPath, previousManifest, currentManifest);

			Assert.Equal(1, result.DeletedCount);
			Assert.False(File.Exists(staleFilePath));
		}
		finally
		{
			if (Directory.Exists(outputPath))
			{
				Directory.Delete(outputPath, true);
			}
		}
	}

	[Fact]
	public async Task CleanupAsync_must_not_touch_untracked_files()
	{
		var outputPath = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(outputPath);
		var customFilePath = Path.Combine(outputPath, "CustomController.cs");
		await File.WriteAllTextAsync(customFilePath, "custom controller");

		try
		{
			var previousManifest = new GeneratedOutputManifest
			{
				Files = ["Generated/Old.txt"]
			};

			var currentManifest = new GeneratedOutputManifest();

			var result = await GeneratedOutputCleaner.CleanupAsync(outputPath, previousManifest, currentManifest);

			Assert.Equal(0, result.DeletedCount);
			Assert.True(File.Exists(customFilePath));
		}
		finally
		{
			if (Directory.Exists(outputPath))
			{
				Directory.Delete(outputPath, true);
			}
		}
	}
}