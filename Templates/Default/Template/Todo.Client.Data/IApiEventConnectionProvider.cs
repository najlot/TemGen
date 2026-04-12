using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data;

public interface IApiEventConnectionProvider
{
	Task<HubConnection> GetConnectionAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>