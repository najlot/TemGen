using Cosei.Client.Base;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;

public interface ILocalSubscriber : ISubscriber
{
	Task SendAsync<T>(T message) where T : class;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>