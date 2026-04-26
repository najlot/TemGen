using Najlot.Map.Attributes;
using System.Linq.Expressions;
using <# Project.Namespace#>.Contracts.Users;

namespace <# Project.Namespace#>.Service.Features.Users;

[Mapping]
internal partial class UserMappings
{
	public static partial void MapToCreated(UserModel from, UserCreated to);

	public static partial void MapToUpdated(UserModel from, UserUpdated to);

	[MapIgnoreProperty(nameof(to.PasswordHash))]
	[MapIgnoreProperty(nameof(to.PasswordResetCodeHash))]
	[MapIgnoreProperty(nameof(to.PasswordResetCodeExpiresAt))]
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