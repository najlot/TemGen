using System;

namespace TemGen;

public sealed class TemplateReadException : Exception
{
	public string TemplatePath { get; }

	public TemplateReadException(string templatePath, string message, Exception innerException)
		: base($"Error reading template '{templatePath}': {message}", innerException)
	{
		TemplatePath = templatePath;
	}
}