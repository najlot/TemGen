using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Todo.Client.Data;

public interface IApiEventConnectionProvider
{
	Task<HubConnection> GetConnectionAsync();
}