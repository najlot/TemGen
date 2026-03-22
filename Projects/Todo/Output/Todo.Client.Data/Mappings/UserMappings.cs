using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Mappings;

[Mapping]
internal sealed partial class UserMappings
{
	public static partial void MapToModel(UserListItem from, UserListItemModel to);

	public static partial void MapToModel(User from, UserModel to);

	public static partial void MapToCreate(UserModel from, CreateUser to);

	public static partial void MapToUpdate(UserModel from, UpdateUser to);
}