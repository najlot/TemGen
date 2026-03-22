using Najlot.Map.Attributes;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Contracts.Commands;
using <# Project.Namespace#>.Contracts.Events;
using <# Project.Namespace#>.Contracts.ListItems;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Mappings;

[Mapping]
internal partial class UserMappings
{
	public static partial void MapToCreated(UserModel from, UserCreated to);

	public static partial void MapToUpdated(UserModel from, UserUpdated to);

	[MapIgnoreProperty(nameof(to.PasswordHash))]
	[MapIgnoreProperty(nameof(to.DeletedAt))]
	public static void MapToModel(CreateUser from, UserModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	[MapIgnoreProperty(nameof(to.Password))]
	public static void MapFromModel(UserModel from, User to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	public static Expression<Func<UserModel, UserListItem>> GetListItemExpression()
	{
		return from => new UserListItem
		{
			Id = from.Id,
			Username = from.Username,
			EMail = from.EMail
		};
	}

	public static void MapFromModel(UserModel from, UserListItem to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>