using System.IO;
using System.Linq;

namespace TemGen;

public static class ProjectReader
{
	public static Project ReadProject(string path)
	{
		var project = new Project();
		if (string.IsNullOrWhiteSpace(path))
		{
			path = "./ProjectDefinition";
		}
		else if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
		{
			path = Path.Combine(path, "ProjectDefinition");
		}

		path = Path.GetFullPath(path);
		project.ProjectDirectory = Path.GetDirectoryName(path);
		var lines = File.ReadAllLines(path).Where(p => !string.IsNullOrWhiteSpace(p) && !p.StartsWith('#'));

		foreach (var line in lines)
		{
			var index = line.IndexOf(':');

			if (index == -1)
			{
				continue;
			}

			var value = line.Substring(index + 1).Trim();

			switch (line.Substring(0, index).Trim())
			{
				case "Namespace":
					project.Namespace = value;
					break;

				case "DefinitionsPath":
					if (!Path.IsPathFullyQualified(value))
					{
						value = Path.GetFullPath(Path.Combine(project.ProjectDirectory, value));
					}

					project.DefinitionsPath = value;
					break;

				case "TemplatesPath":
					if (!Path.IsPathFullyQualified(value))
					{
						value = Path.GetFullPath(Path.Combine(project.ProjectDirectory, value));
					}

					project.TemplatesPath = value;
					break;

				case "OutputPath":
					if (!Path.IsPathFullyQualified(value))
					{
						value = Path.GetFullPath(Path.Combine(project.ProjectDirectory, value));
					}

					project.OutputPath = value;
					break;

				case "ResourcesPath":
					if (!Path.IsPathFullyQualified(value))
					{
						value = Path.GetFullPath(Path.Combine(project.ProjectDirectory, value));
					}

					project.ResourcesPath = value;
					break;

				case "ResourcesScriptPath":
					if (!Path.IsPathFullyQualified(value))
					{
						value = Path.GetFullPath(Path.Combine(project.ProjectDirectory, value));
					}

					project.ResourcesScriptPath = value;
					break;

				case "PrimaryColor":
					project.PrimaryColor = value;
					break;

				case "PrimaryDarkColor":
					project.PrimaryDarkColor = value;
					break;

				case "AccentColor":
					project.AccentColor = value;
					break;

				case "ForegroundColor":
					project.ForegroundColor = value;
					break;
			}
		}

		return project;
	}
}