using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.ListItems;

namespace <# Project.Namespace#>.Client.Data.Mappings;

[Mapping]
internal sealed partial class UserMappings
{
	public static partial void MapToModel(UserListItem from, UserListItemModel to);

	public static partial void MapToModel(User from, UserModel to);

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
}<#cs SetOutputPathAndSkipOtherDefinitions()#>