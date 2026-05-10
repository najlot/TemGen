using System;
using <# Project.Namespace#>.Client.Localisation;

namespace <# Project.Namespace#>.Client.Data.Users;

public class UserListItemModel
{
	public Guid Id { get; set; }

	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;

	public string DisplayText => Id == Guid.Empty ? CommonLoc.NothingSelected : Username;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>