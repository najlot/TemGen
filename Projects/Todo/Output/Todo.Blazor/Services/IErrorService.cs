namespace Todo.Blazor.Services;

public interface IErrorService
{
	void ShowError(string title, string message);
	void HideError();
}