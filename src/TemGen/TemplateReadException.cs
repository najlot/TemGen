using System;

namespace TemGen;

public sealed class TemplateReadException(string templatePath, string message, Exception innerException)
	: Exception($"Error reading template '{templatePath}': {message}", innerException)
{
	public string TemplatePath { get; } = templatePath;
}