using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.Contracts.Users;


namespace Todo.Service.Features.Users;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(List<UserListItem>))]
[JsonSerializable(typeof(UserListItem[]))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(UpdateUser))]
[JsonSerializable(typeof(UserModel))]
[JsonSerializable(typeof(UserCreated))]
[JsonSerializable(typeof(UserUpdated))]
[JsonSerializable(typeof(UserDeleted))]
public partial class UserSerializerContext : JsonSerializerContext
{
}
