foreach (var path in Directory.GetFiles(Project.ResourcesPath, "*.*", SearchOption.AllDirectories))
{
	var newPath = path.Replace(Project.ResourcesPath, Project.OutputPath).Replace("Project.Namespace", Project.Namespace);
	Directory.CreateDirectory(Path.GetDirectoryName(newPath));

	// Validate modify time of the file, if the file is not modified, skip copying the file to save time
	var lastWriteTime = File.GetLastWriteTime(path);
	if (!File.Exists(newPath) || lastWriteTime != File.GetLastWriteTime(newPath))
	{
		File.Copy(path, newPath, true);
		File.SetLastWriteTime(newPath, lastWriteTime);
	}
}