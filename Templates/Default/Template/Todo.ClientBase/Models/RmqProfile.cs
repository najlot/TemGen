namespace <#cs Write(Project.Namespace)#>.ClientBase.Models;

public class RmqProfile : ProfileBase
{
	public string ServerUser { get; set; }
	public string ServerPassword { get; set; }

	public string RabbitMqHost { get; set; }
	public string RabbitMqVirtualHost { get; set; }
	public string RabbitMqPassword { get; set; }
	public string RabbitMqUser { get; set; }
}<#cs SetOutputPathAndSkipOtherDefinitions()#>