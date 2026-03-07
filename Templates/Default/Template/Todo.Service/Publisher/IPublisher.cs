using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Service.Publisher;

public interface IPublisher
{
	Task PublishAsync<T>(T message) where T : class;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
