namespace Todo.ClientBase.Models;

public class RestProfile : ProfileBase
{
	public string ServerName { get; set; }
	public string ServerUser { get; set; }
	public string ServerPassword { get; set; }
}