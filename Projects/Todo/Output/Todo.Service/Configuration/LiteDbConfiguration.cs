namespace Todo.Service.Configuration;

public sealed class LiteDbConfiguration
{
	public string DatabasePath { get; set; } = "Data/Todo.db";
}