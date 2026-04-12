using System;

namespace Todo.Client.Data.Identity;

public sealed class SessionUnavailableException : InvalidOperationException
{
	public SessionUnavailableException()
		: base("No valid session is available.")
	{
	}
}
