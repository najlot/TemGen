using Najlot.Map.Attributes;
using System;
using System.Linq.Expressions;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Mappings;

[Mapping]
internal partial class UserMappings
{
	public static UserCreated MapToCreated(UserModel from) =>
		new(from.Id,
		from.Username,
		from.EMail);

	public static UserUpdated MapToUpdated(UserModel from) =>
		new(from.Id,
		from.Username,
		from.EMail);

	[MapIgnoreProperty(nameof(to.PasswordHash))]
	[MapIgnoreProperty(nameof(to.IsActive))]
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