using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Components;

namespace <# Project.Namespace#>.Blazor.Pages;

public static class NavigationManagerExtensions
{
	public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T? value)
	{
		var uri = navManager.ToAbsoluteUri(navManager.Uri);

		if (TryGetQueryValue(uri.Query, key, out var valueFromQueryString))
		{
			if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
			{
				value = (T)(object)valueAsInt;
				return true;
			}

			if (typeof(T) == typeof(string))
			{
				value = (T)(object)valueFromQueryString.ToString();
				return true;
			}

			if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueAsDecimal))
			{
				value = (T)(object)valueAsDecimal;
				return true;
			}
		}

		value = default;
		return false;
	}

	public static bool TryGetReturnUrl(this NavigationManager navManager, out string? value)
	{
		return navManager.TryGetQueryString("ReturnUrl", out value);
	}

	public static string BuildReturnUrl(this NavigationManager navManager, string relativeTo)
	{
		var uri = navManager.ToBaseRelativePath(navManager.Uri);

		if (string.IsNullOrWhiteSpace(uri))
		{
			return relativeTo;
		}

		var separator = relativeTo.Contains('?', StringComparison.Ordinal) ? '&' : '?';
		return $"{relativeTo}{separator}ReturnUrl={Uri.EscapeDataString(uri)}";
	}

	private static bool TryGetQueryValue(string query, string key, out string value)
	{
		value = string.Empty;

		if (string.IsNullOrWhiteSpace(query))
		{
			return false;
		}

		foreach (var segment in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
		{
			var parts = segment.Split('=', 2);
			var currentKey = WebUtility.UrlDecode(parts[0]);

			if (!string.Equals(currentKey, key, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			value = parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) ?? string.Empty : string.Empty;
			return true;
		}

		return false;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>