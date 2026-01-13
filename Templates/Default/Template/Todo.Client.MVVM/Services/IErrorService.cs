using System;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM.Services;

public interface IErrorService
{
	Task ShowAlertAsync(Exception ex);
	Task ShowAlertAsync(string message, Exception ex);
	Task ShowAlertAsync(string title, string message);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>