using System;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Models;

public abstract class ProfileBase
{
	public Guid Id { get; set; }
	public Source Source { get; set; }
	public string Name { get; set; }

	public ProfileBase Clone()
	{
		return MemberwiseClone() as ProfileBase;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>