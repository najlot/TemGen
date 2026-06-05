using System;

namespace <# Project.Namespace#>.Client.Data.Identity;

public sealed class SessionUnavailableException : InvalidOperationException
{
	public SessionUnavailableException()
		: base("No valid session is available.")
	{
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>