namespace Todo.Service.Configuration;

public class MySqlConfiguration
{
	public string Host { get; set; } = "";
	public int Port { get; set; } = 3306;
	public string Database { get; set; } = "";
	public string User { get; set; } = "";
	public string Password { get; set; } = "";
}