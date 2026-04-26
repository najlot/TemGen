using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Auth;

namespace <# Project.Namespace#>.Service.Features.Auth;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(RequestPasswordReset))]
[JsonSerializable(typeof(ResetPassword))]
public partial class AuthSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>