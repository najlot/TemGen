using Najlot.Map.Attributes;
using Todo.Contracts.Users;

namespace Todo.Client.Data.Users;

[Mapping]
internal sealed partial class UserMappings
{
	public static partial void MapToModel(UserListItem from, UserListItemModel to);

	public static partial void MapToModel(User from, UserModel to);

	public static partial void MapToCreate(UserModel from, CreateUser to);

	public static partial void MapToUpdate(UserModel from, UpdateUser to);
}