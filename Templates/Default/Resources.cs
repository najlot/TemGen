foreach (var path in Directory.GetFiles(Project.ResourcesPath, "*.*", SearchOption.AllDirectories))
{
	var newPath = path.Replace(Project.ResourcesPath, Project.OutputPath).Replace("Project.Namespace", Project.Namespace);
	var directoryName = Path.GetDirectoryName(newPath);
	if (!string.IsNullOrEmpty(directoryName))
	{
		Directory.CreateDirectory(directoryName);
	}

	// Validate modify time of the file, if the file is not modified, skip copying the file to save time
	var lastWriteTime = File.GetLastWriteTime(path);
	if (!File.Exists(newPath) || lastWriteTime != File.GetLastWriteTime(newPath))
	{
		File.Copy(path, newPath, true);
		File.SetLastWriteTime(newPath, lastWriteTime);
	}
}