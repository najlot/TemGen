foreach (var path in Directory.GetFiles(Project.ResourcesPath, "*.*", SearchOption.AllDirectories))
{
	var newPath = path.Replace(Project.ResourcesPath, Project.OutputPath).Replace("Project.Namespace", Project.Namespace);
	var dirPath = Path.GetDirectoryName(newPath);
	Directory.CreateDirectory(dirPath);
	File.Copy(path, newPath, true);
}