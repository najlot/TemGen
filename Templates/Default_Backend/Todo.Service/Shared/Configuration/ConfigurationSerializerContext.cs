using System.Text.Json;
using System.Text.Json.Serialization;

namespace <# Project.Namespace#>.Service.Shared.Configuration;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(StorageConfiguration))]
[JsonSerializable(typeof(BackupConfiguration))]
[JsonSerializable(typeof(FileConfiguration))]
[JsonSerializable(typeof(LiteDbConfiguration))]
[JsonSerializable(typeof(MongoDbConfiguration))]
[JsonSerializable(typeof(MySqlConfiguration))]
[JsonSerializable(typeof(ServiceConfiguration))]
[JsonSerializable(typeof(SmtpConfiguration))]
public partial class ConfigurationSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>