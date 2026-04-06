namespace Todo.Service.Shared.Configuration;

public sealed class LiteDbConfiguration
{
	public string DatabasePath { get; set; } = "Data/Todo.db";
}