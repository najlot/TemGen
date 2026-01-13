using Cosei.Client.Base;

namespace Todo.Blazor.Services;

public interface ISubscriberProvider
{
	Task<ISubscriber> GetSubscriber();
	Task ClearSubscriber();
}