using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Mappings;

internal sealed class UserMappings
{
	public void MapToModel(UserListItem from, UserListItemModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	public void MapToModel(User from, UserModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
		to.Password = from.Password;
	}

	public CreateUser MapToCreate(UserModel item) =>
		new(item.Id,
			item.Username,
			item.EMail,
			item.Password);

	public UpdateUser MapToUpdate(UserModel item) =>
		new(item.Id,
			item.Username,
			item.EMail,
			item.Password);
}