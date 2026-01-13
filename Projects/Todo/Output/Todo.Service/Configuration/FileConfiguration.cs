namespace Todo.Service.Configuration;

public class FileConfiguration
{
	public string UsersPath { get; set; } = "Data/Users";
	public string NotesPath { get; set; } = "Data/Notes";
	public string TodoItemsPath { get; set; } = "Data/TodoItems";
}