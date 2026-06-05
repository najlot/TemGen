using Markdig;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using <# Project.Namespace#>.Service.Shared.Configuration;

namespace <# Project.Namespace#>.Service.Features.Auth;

public class SmtpPasswordResetCodeSender(
	SmtpConfiguration smtpConfiguration,
	ILogger<SmtpPasswordResetCodeSender> logger) : IPasswordResetCodeSender
{
	private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

	private const string DefaultSubject = "Password reset code";
	private const string ResetPasswordTemplateName = "ResetPassword";

	public bool CanSend =>
		!string.IsNullOrWhiteSpace(smtpConfiguration.Host)
		&& !string.IsNullOrWhiteSpace(smtpConfiguration.MailFromAddress);

	public async Task SendAsync(string email, string username, string code, TimeSpan expiresIn, string culture)
	{
		if (!CanSend)
		{
			throw new InvalidOperationException("Password reset is currently unavailable.");
		}

		var markdownBody = await LoadAndRenderTemplateAsync(username, code, expiresIn, culture).ConfigureAwait(false);
		var subject = GetSubject(markdownBody);
		var htmlBody = Markdown.ToHtml(markdownBody, _pipeline);

		using var message = new MailMessage
		{
			From = CreateFromAddress(),
			Subject = subject,
			SubjectEncoding = Encoding.UTF8,
			Body = htmlBody,
			BodyEncoding = Encoding.UTF8,
			IsBodyHtml = true
		};

		message.To.Add(email);

		using var client = CreateClient();

		try
		{
			await client.SendMailAsync(message).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to send password reset code to {Email}", email);
			throw;
		}
	}

	private MailAddress CreateFromAddress()
	{
		return string.IsNullOrWhiteSpace(smtpConfiguration.MailFromName)
			? new MailAddress(smtpConfiguration.MailFromAddress)
			: new MailAddress(smtpConfiguration.MailFromAddress, smtpConfiguration.MailFromName);
	}

	private SmtpClient CreateClient()
	{
		var client = new SmtpClient(smtpConfiguration.Host, smtpConfiguration.Port)
		{
			EnableSsl = smtpConfiguration.UseSsl,
			DeliveryMethod = SmtpDeliveryMethod.Network,
			UseDefaultCredentials = false,
		};

		if (!string.IsNullOrWhiteSpace(smtpConfiguration.Username))
		{
			client.Credentials = new NetworkCredential(smtpConfiguration.Username, smtpConfiguration.Password);
		}

		return client;
	}

	private static string GetSubject(string markdownBody)
	{
		using var reader = new StringReader(markdownBody);

		while (reader.ReadLine() is { } line)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}

			if (line.StartsWith("# ", StringComparison.Ordinal))
			{
				return line[2..].Trim();
			}

			break;
		}

		return DefaultSubject;
	}

	private static async Task<string> LoadAndRenderTemplateAsync(string username, string code, TimeSpan expiresIn, string culture)
	{
		var template = await LoadTemplateAsync(ResetPasswordTemplateName, culture).ConfigureAwait(false);
		var expiresInMinutes = (int)Math.Ceiling(expiresIn.TotalMinutes);

		return ReplacePlaceholders(template, new Dictionary<string, string>
		{
			["Username"] = EscapeMarkdown(username),
			["Code"] = EscapeMarkdown(code),
			["ExpiresInMinutes"] = expiresInMinutes.ToString(CultureInfo.InvariantCulture),
		});
	}

	private static async Task<string> LoadTemplateAsync(string templateName, string culture)
	{
		foreach (var templatePath in GetTemplatePaths(templateName, culture))
		{
			if (File.Exists(templatePath))
			{
				return await File.ReadAllTextAsync(templatePath).ConfigureAwait(false);
			}
		}

		throw new FileNotFoundException($"No template file found for '{templateName}'.");
	}

	private static IEnumerable<string> GetTemplatePaths(string templateName, string culture)
	{
		var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates");
		var normalizedCulture = NormalizeCulture(culture);

		if (!string.IsNullOrWhiteSpace(normalizedCulture))
		{
			yield return Path.Combine(templatesPath, $"{templateName}.{normalizedCulture}.md");

			var separatorIndex = normalizedCulture.IndexOf('-');
			if (separatorIndex > 0)
			{
				var neutralCulture = normalizedCulture[..separatorIndex];
				yield return Path.Combine(templatesPath, $"{templateName}.{neutralCulture}.md");
			}
		}

		yield return Path.Combine(templatesPath, $"{templateName}.en.md");
		yield return Path.Combine(templatesPath, $"{templateName}.md");
	}

	private static string NormalizeCulture(string culture)
	{
		if (string.IsNullOrWhiteSpace(culture))
		{
			return string.Empty;
		}

		var normalizedCulture = culture.Trim().Replace('_', '-');

		try
		{
			return CultureInfo.GetCultureInfo(normalizedCulture).Name;
		}
		catch (CultureNotFoundException)
		{
			return normalizedCulture;
		}
	}

	private static string ReplacePlaceholders(string template, IReadOnlyDictionary<string, string> placeholders)
	{
		var renderedTemplate = template;

		foreach (var placeholder in placeholders)
		{
			renderedTemplate = renderedTemplate.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value, StringComparison.Ordinal);
		}

		return renderedTemplate;
	}

	private static string EscapeMarkdown(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}

		var escapedValue = new StringBuilder(value.Length);

		foreach (var ch in value)
		{
			if (ch is '\\' or '`' or '*' or '_' or '{' or '}' or '[' or ']' or '(' or ')' or '#' or '+' or '-' or '!' or '|')
			{
				escapedValue.Append('\\');
			}

			escapedValue.Append(ch);
		}

		return escapedValue.ToString();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>