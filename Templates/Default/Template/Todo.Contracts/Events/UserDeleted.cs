using System;

namespace <#cs Write(Project.Namespace)#>.Contracts.Events;

public class UserDeleted(Guid id)
{
	public Guid Id { get; } = id;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>