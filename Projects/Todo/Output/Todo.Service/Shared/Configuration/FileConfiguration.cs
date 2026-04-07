namespace Todo.Service.Shared.Configuration;

public class FileConfiguration
{
	public string UsersPath { get; set; } = "Data/Users";
	public string HistoryPath { get; set; } = "Data/History";
	public string NotesPath { get; set; } = "Data/Notes";
	public string TodoItemsPath { get; set; } = "Data/TodoItems";
}