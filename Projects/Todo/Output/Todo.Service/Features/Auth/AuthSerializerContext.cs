using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Auth;

namespace Todo.Service.Features.Auth;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(RequestPasswordReset))]
[JsonSerializable(typeof(ResetPassword))]
public partial class AuthSerializerContext : JsonSerializerContext
{
}
