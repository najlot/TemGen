using Cosei.Client.Base;

namespace <#cs Write(Project.Namespace)#>.Blazor.Services;

public interface ISubscriberProvider
{
	Task<ISubscriber> GetSubscriber();
	Task ClearSubscriber();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>