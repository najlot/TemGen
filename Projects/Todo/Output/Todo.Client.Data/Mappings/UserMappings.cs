using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Mappings;

[Mapping]
internal sealed partial class UserMappings
{
	public static void MapToModel(UserListItem from, UserListItemModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	public static void MapToModel(User from, UserModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
		to.Password = from.Password;
	}

	public static CreateUser MapToCreate(UserModel item) =>
		new(item.Id,
			item.Username,
			item.EMail,
			item.Password);

	public static UpdateUser MapToUpdate(UserModel item) =>
		new(item.Id,
			item.Username,
			item.EMail,
			item.Password);
}