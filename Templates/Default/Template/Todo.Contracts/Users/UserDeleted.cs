using System;

namespace <# Project.Namespace#>.Contracts.Users;

public class UserDeleted(Guid id)
{
	public Guid Id { get; } = id;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>