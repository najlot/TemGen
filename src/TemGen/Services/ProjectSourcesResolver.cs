using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TemGen.Models;

namespace TemGen.Services;

public sealed class ProjectSourcesResolver : IDisposable
{
	private const string TemplateDirectoryName = "Template";
	private const string ScriptsDirectoryName = "Scripts";
	private const string ResourcesScriptPattern = "Resources.*";

	private readonly Dictionary<(string RepositoryUrl, string RefName), string> _gitCheckouts = new();
	private readonly string _tempRoot = Path.Combine(Path.GetTempPath(), "TemGen", "Sources", Guid.NewGuid().ToString("N"));
	private bool _disposed;

	public ResolvedProjectSources Resolve(Project project)
	{
		var templateSources = project.TemplatePaths
			.Select(ResolveTemplateSource)
			.ToList();

		return new ResolvedProjectSources
		{
			DefinitionsPath = MaterializePath(project.DefinitionsPath),
			TemplatePaths = DistinctPaths(templateSources.Select(source => source.TemplatePath)).ToArray(),
			ScriptPaths = ResolveScriptPaths(project.ScriptsPath, templateSources).ToArray(),
			ResourceScriptPaths = ResolveResourceScriptPaths(project.ResourcesScriptPath, templateSources).ToArray()
		};
	}

	public string MaterializePath(string sourcePath)
	{
		if (string.IsNullOrWhiteSpace(sourcePath))
		{
			return sourcePath;
		}

		if (TryParseGitSource(sourcePath, out var gitSource))
		{
			var checkoutPath = GetOrCreateCheckout(gitSource.RepositoryUrl, gitSource.RefName);

			if (string.IsNullOrWhiteSpace(gitSource.SubPath))
			{
				return checkoutPath;
			}

			var relativePath = gitSource.SubPath.Replace('/', Path.DirectorySeparatorChar);
			return Path.GetFullPath(Path.Combine(checkoutPath, relativePath));
		}

		if (TryGetFileUriPath(sourcePath, out var localPath))
		{
			return localPath;
		}

		return sourcePath;
	}

	public static bool IsSourceUri(string path)
	{
		if (!Uri.TryCreate(path, UriKind.Absolute, out var uri))
		{
			return false;
		}

		return uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
			|| uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
			|| uri.Scheme.Equals(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)
			|| uri.Scheme.Equals("ssh", StringComparison.OrdinalIgnoreCase);
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;

		try
		{
			if (Directory.Exists(_tempRoot))
			{
				Directory.Delete(_tempRoot, true);
			}
		}
		catch
		{
			// Best-effort cleanup for temporary git checkouts.
		}
	}

	private static IEnumerable<string> DistinctPaths(IEnumerable<string> paths)
	{
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		foreach (var path in paths)
		{
			if (!string.IsNullOrWhiteSpace(path) && seen.Add(path))
			{
				yield return path;
			}
		}
	}

	private ResolvedTemplateSource ResolveTemplateSource(string sourcePath)
	{
		var localPath = MaterializePath(sourcePath);
		var templatePath = localPath;
		string sourceRoot = null;

		if (!string.IsNullOrWhiteSpace(localPath) && Directory.Exists(localPath))
		{
			var nestedTemplatePath = Path.Combine(localPath, TemplateDirectoryName);

			if (Directory.Exists(nestedTemplatePath))
			{
				templatePath = nestedTemplatePath;
				sourceRoot = localPath;
			}
			else if (IsNamedDirectory(localPath, TemplateDirectoryName))
			{
				sourceRoot = Directory.GetParent(localPath)?.FullName;
			}
		}

		return new ResolvedTemplateSource(templatePath, sourceRoot);
	}

	private IEnumerable<string> ResolveScriptPaths(string explicitScriptsPath, IReadOnlyList<ResolvedTemplateSource> templateSources)
	{
		var explicitPaths = Project.SplitPaths(explicitScriptsPath)
			.Select(ResolveScriptPath)
			.ToArray();

		if (explicitPaths.Length > 0)
		{
			return DistinctPaths(explicitPaths);
		}

		return DistinctPaths(templateSources
			.Where(source => !string.IsNullOrWhiteSpace(source.SourceRoot))
			.Select(source => Path.Combine(source.SourceRoot, ScriptsDirectoryName))
			.Where(Directory.Exists));
	}

	private string ResolveScriptPath(string sourcePath)
	{
		var localPath = MaterializePath(sourcePath);

		if (!string.IsNullOrWhiteSpace(localPath) && Directory.Exists(localPath))
		{
			var nestedScriptsPath = Path.Combine(localPath, ScriptsDirectoryName);

			if (Directory.Exists(nestedScriptsPath))
			{
				return nestedScriptsPath;
			}
		}

		return localPath;
	}

	private IEnumerable<string> ResolveResourceScriptPaths(string explicitResourcesScriptPath, IReadOnlyList<ResolvedTemplateSource> templateSources)
	{
		var explicitPaths = Project.SplitPaths(explicitResourcesScriptPath)
			.SelectMany(ResolveResourceScriptPathsFromSource)
			.ToArray();

		if (explicitPaths.Length > 0)
		{
			return DistinctPaths(explicitPaths);
		}

		return DistinctPaths(templateSources
			.Where(source => !string.IsNullOrWhiteSpace(source.SourceRoot))
			.SelectMany(source => FindResourceScripts(source.SourceRoot)));
	}

	private IEnumerable<string> ResolveResourceScriptPathsFromSource(string sourcePath)
	{
		var localPath = MaterializePath(sourcePath);

		if (!string.IsNullOrWhiteSpace(localPath) && Directory.Exists(localPath))
		{
			var resourceScripts = FindResourceScripts(localPath).ToArray();

			if (resourceScripts.Length > 0)
			{
				return resourceScripts;
			}
		}

		return string.IsNullOrWhiteSpace(localPath)
			? Enumerable.Empty<string>()
			: new[] { localPath };
	}

	private static IEnumerable<string> FindResourceScripts(string directoryPath)
	{
		if (!Directory.Exists(directoryPath))
		{
			return Enumerable.Empty<string>();
		}

		return Directory
			.GetFiles(directoryPath, ResourcesScriptPattern, SearchOption.TopDirectoryOnly)
			.OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private string GetOrCreateCheckout(string repositoryUrl, string refName)
	{
		var cacheKey = (repositoryUrl, refName ?? string.Empty);

		if (_gitCheckouts.TryGetValue(cacheKey, out var checkoutPath))
		{
			return checkoutPath;
		}

		Directory.CreateDirectory(_tempRoot);
		checkoutPath = Path.Combine(_tempRoot, $"{_gitCheckouts.Count:D2}_{Guid.NewGuid():N}");

		var arguments = new List<string>
		{
			"clone",
			"--depth",
			"1"
		};

		if (!string.IsNullOrWhiteSpace(refName))
		{
			arguments.Add("--branch");
			arguments.Add(refName);
		}

		arguments.Add(repositoryUrl);
		arguments.Add(checkoutPath);

		RunGit(arguments, Directory.GetCurrentDirectory());
		_gitCheckouts[cacheKey] = checkoutPath;
		return checkoutPath;
	}

	private static void RunGit(IEnumerable<string> arguments, string workingDirectory)
	{
		var startInfo = new ProcessStartInfo("git")
		{
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = workingDirectory
		};

		foreach (var argument in arguments)
		{
			startInfo.ArgumentList.Add(argument);
		}

		using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start the git process.");
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

	private static bool TryParseGitSource(string sourcePath, out GitSourceDescriptor source)
	{
		if (!Uri.TryCreate(sourcePath, UriKind.Absolute, out var uri))
		{
			source = null;
			return false;
		}

		return TryParseGitHubSource(uri, out source)
			|| TryParseGenericGitSource(uri, out source);
	}

	private static bool TryParseGitHubSource(Uri uri, out GitSourceDescriptor source)
	{
		source = null;

		if ((!uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
				&& !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
			|| !uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		var segments = uri.AbsolutePath
			.Trim('/')
			.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		if (segments.Length < 4
			|| !segments[2].Equals("tree", StringComparison.OrdinalIgnoreCase) && !segments[2].Equals("blob", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		var owner = Uri.UnescapeDataString(segments[0]);
		var repository = Uri.UnescapeDataString(segments[1]);
		var refName = Uri.UnescapeDataString(segments[3]);
		var subPath = segments.Length > 4
			? string.Join('/', segments.Skip(4).Select(Uri.UnescapeDataString))
			: string.Empty;

		source = new GitSourceDescriptor($"{uri.Scheme}://{uri.Authority}/{owner}/{repository}.git", refName, NormalizeGitSubPath(subPath));
		return true;
	}

	private static bool TryParseGenericGitSource(Uri uri, out GitSourceDescriptor source)
	{
		source = null;

		if (!uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
			&& !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
			&& !uri.Scheme.Equals(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)
			&& !uri.Scheme.Equals("ssh", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		var query = ParseQuery(uri.Query);
		var hasGitQuery = query.ContainsKey("ref") || query.ContainsKey("path");

		if (!uri.AbsolutePath.EndsWith(".git", StringComparison.OrdinalIgnoreCase) && !hasGitQuery)
		{
			return false;
		}

		query.TryGetValue("ref", out var refName);
		query.TryGetValue("path", out var subPath);
		var repositoryUrl = uri.GetLeftPart(UriPartial.Path);
		source = new GitSourceDescriptor(repositoryUrl, NormalizeGitValue(refName), NormalizeGitSubPath(subPath));
		return true;
	}

	private static Dictionary<string, string> ParseQuery(string query)
	{
		var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		if (string.IsNullOrWhiteSpace(query))
		{
			return values;
		}

		foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
		{
			var separatorIndex = pair.IndexOf('=');
			var key = separatorIndex >= 0 ? pair[..separatorIndex] : pair;
			var value = separatorIndex >= 0 ? pair[(separatorIndex + 1)..] : string.Empty;
			values[Uri.UnescapeDataString(key)] = Uri.UnescapeDataString(value);
		}

		return values;
	}

	private static string NormalizeGitValue(string value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
	}

	private static string NormalizeGitSubPath(string subPath)
	{
		subPath = NormalizeGitValue(subPath);

		return string.IsNullOrWhiteSpace(subPath)
			? null
			: subPath.Trim('/').Replace('\\', '/');
	}

	private static bool TryGetFileUriPath(string sourcePath, out string localPath)
	{
		if (Uri.TryCreate(sourcePath, UriKind.Absolute, out var uri) && uri.IsFile)
		{
			localPath = uri.LocalPath;
			return true;
		}

		localPath = null;
		return false;
	}

	private static bool IsNamedDirectory(string directoryPath, string name)
	{
		return Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
			.Equals(name, StringComparison.OrdinalIgnoreCase);
	}

	private sealed record GitSourceDescriptor(string RepositoryUrl, string RefName, string SubPath);
	private sealed record ResolvedTemplateSource(string TemplatePath, string SourceRoot);
}

public sealed class ResolvedProjectSources
{
	public string DefinitionsPath { get; init; }
	public IReadOnlyList<string> TemplatePaths { get; init; } = [];
	public IReadOnlyList<string> ScriptPaths { get; init; } = [];
	public IReadOnlyList<string> ResourceScriptPaths { get; init; } = [];
}