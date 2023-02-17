namespace TemGen.Handler;

public class HandlingResult
{
	public string RelativePath { get; set; }
	public string Content { get; set; }
	public bool SkipOtherDefinitions { get; set; }
	public bool RepeatForEachDefinitionEntry { get; set; }
}