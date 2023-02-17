namespace TemGen;

public class Project
{
	public string Namespace { get; set; }

	public string ProjectDirectory { get; set; }
	public string DefinitionsPath { get; set; }
	public string TemplatesPath { get; set; }
	public string OutputPath { get; set; }
	public string ResourcesPath { get; set; }
	public string ResourcesScriptPath { get; set; }

	public string PrimaryColor { get; set; }
	public string PrimaryDarkColor { get; set; }
	public string AccentColor { get; set; }
	public string ForegroundColor { get; set; }
}