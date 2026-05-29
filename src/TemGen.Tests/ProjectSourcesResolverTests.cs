using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace TemGen.Tests;

public class ProjectSourcesResolverTests
{
	[Fact]
	public void Resolve_materializes_git_sources_and_discovers_template_scripts_and_resources()
	{
		var repositoryDirectory = Path.Combine(Path.GetTempPath(), "TemGen.Tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(repositoryDirectory);

		try
		{
			Directory.CreateDirectory(Path.Combine(repositoryDirectory, "Templates", "Default", "Template"));
			Directory.CreateDirectory(Path.Combine(repositoryDirectory, "Templates", "Default", "Scripts"));

			File.WriteAllText(Path.Combine(repositoryDirectory, "Templates", "Default", "Template", "Entity.txt"), "Hello");
			File.WriteAllText(Path.Combine(repositoryDirectory, "Templates", "Default", "Scripts", "Common.cs"), "WriteLine(\"Script\");");
			File.WriteAllText(Path.Combine(repositoryDirectory, "Templates", "Default", "Resources.cs"), "WriteLine(\"Resources\");");

			RunGit(repositoryDirectory, "init");
			RunGit(repositoryDirectory, "checkout", "-b", "main");
			RunGit(repositoryDirectory, "config", "user.email", "temgen-tests@example.com");
			RunGit(repositoryDirectory, "config", "user.name", "TemGen Tests");
			RunGit(repositoryDirectory, "add", ".");
			RunGit(repositoryDirectory, "commit", "-m", "Initial commit");

			var project = new Project
			{
				DefinitionsPath = repositoryDirectory,
				TemplatesPath = $"{new Uri(repositoryDirectory + Path.DirectorySeparatorChar).AbsoluteUri}?ref=main&path=Templates/Default"
			};

			using var resolver = new ProjectSourcesResolver();
			var sources = resolver.Resolve(project);

			Assert.Single(sources.TemplatePaths);
			Assert.Single(sources.ScriptPaths);
			Assert.Single(sources.ResourceScriptPaths);
			Assert.True(File.Exists(Path.Combine(sources.TemplatePaths[0], "Entity.txt")));
			Assert.True(File.Exists(Path.Combine(sources.ScriptPaths[0], "Common.cs")));
			Assert.True(File.Exists(sources.ResourceScriptPaths[0]));
		}
		finally
		{
			DeleteDirectory(repositoryDirectory);
		}
	}

	private static void DeleteDirectory(string directoryPath)
	{
		if (!Directory.Exists(directoryPath))
		{
			return;
		}

		foreach (var filePath in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
		{
			File.SetAttributes(filePath, FileAttributes.Normal);
		}

		Directory.Delete(directoryPath, true);
	}

	private static void RunGit(string workingDirectory, params string[] arguments)
	{
		var startInfo = new ProcessStartInfo("git")
		{
			WorkingDirectory = workingDirectory,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		foreach (var argument in arguments)
		{
			startInfo.ArgumentList.Add(argument);
		}

		using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start git for the test repository.");
		var standardOutput = process.StandardOutput.ReadToEnd();
		var standardError = process.StandardError.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode == 0)
		{
			return;
		}

		var message = string.Join(Environment.NewLine, new[] { standardError, standardOutput }.Where(output => !string.IsNullOrWhiteSpace(output)));
		throw new InvalidOperationException($"git {string.Join(' ', arguments)} failed with exit code {process.ExitCode}.{Environment.NewLine}{message}");
	}
}