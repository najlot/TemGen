using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Users;

namespace Todo.Client.Data.Users;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(UserListItem[]))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(UserCreated))]
[JsonSerializable(typeof(UpdateUser))]
[JsonSerializable(typeof(UserUpdated))]
[JsonSerializable(typeof(UserDeleted))]
public partial class UserSerializerContext : JsonSerializerContext
{
}
