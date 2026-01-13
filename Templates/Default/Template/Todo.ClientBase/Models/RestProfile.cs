namespace <#cs Write(Project.Namespace)#>.ClientBase.Models;

public class RestProfile : ProfileBase
{
	public string ServerName { get; set; }
	public string ServerUser { get; set; }
	public string ServerPassword { get; set; }
}<#cs SetOutputPathAndSkipOtherDefinitions()#>