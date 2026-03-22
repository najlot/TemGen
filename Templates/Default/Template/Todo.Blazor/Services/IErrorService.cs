namespace <# Project.Namespace#>.Blazor.Services;

public interface IErrorService
{
	void ShowError(string title, string message);
	void HideError();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>