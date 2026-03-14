namespace Todo.Service.Configuration;

public class FileConfiguration
{
	public string UsersPath { get; set; } = "Data/Users";
	public string TodoItemsPath { get; set; } = "Data/TodoItems";
	public string NotesPath { get; set; } = "Data/Notes";
}