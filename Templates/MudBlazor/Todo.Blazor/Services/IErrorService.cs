namespace <# Project.Namespace#>.Blazor.Services;

public interface IErrorService
{
	void ShowError(string title, string message);
	void HideError();
	void ShowSuccess(string title, string message, int durationMilliseconds = 4000);
	void HideSuccess();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>