using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Auth;

namespace Todo.Client.Data.Identity;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(RequestPasswordReset))]
[JsonSerializable(typeof(ResetPassword))]
public partial class AuthSerializerContext : JsonSerializerContext
{
}
